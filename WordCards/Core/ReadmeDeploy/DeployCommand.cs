using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using WordCards.Core.Commands;
using WordCards.Core.Contexts;
using WordCards.Core.Helpers;
using WordCards.Extensions;

namespace WordCards.Core.ReadmeDeploy
{
    public sealed partial class DeployCommand : EntityCommand, IDeployCommand
    {
        public void Execute()
        {
            using (var db = new StartContext())
            {
                var infos = GetDeployFileInfos(db.Deploys);

                if (infos.Count > 0)
                {
                    foreach (var info in infos)
                    {
                        ExecuteDeployFile(db, info);
                    }
                }
            }
        }

        private static IReadOnlyList<DeployFileInfo> GetDeployFileInfos(DbSet<ReadmeDeploy> deploys)
        {
            var newFiles = DeployFilesHelper.GetNewFiles(deploys);

            if (newFiles.Count == 0)
            {
                return new List<DeployFileInfo>(0);
            }

            var xmlFiles = GetXmlFiles(newFiles);

            return xmlFiles.Select(x => GetDeployFileInfo(x.Key, x.Value)).ToList();
        }

        private static Dictionary<string, FileInfo> GetXmlFiles(IReadOnlyDictionary<string, string> newFiles)
        {
            return newFiles.Select(x => new
            {
                x.Key,
                File = new FileInfo(x.Value)
            })
                .ToDictionary(x => x.Key, x => x.File);
        }

        private static DeployFileInfo GetDeployFileInfo(string fileId, FileInfo file)
        {
            var doc = new XmlDocument();
            doc.Load(file.OpenText());

            var root = doc.DocumentElement;

            if (root == null)
            {
                return new DeployFileInfo(fileId);
            }

            var id = root.Attributes.GetNamedItem("id")?.Value;

            if (fileId != id)
            {
                throw new Exception($"Wrong File. File Id = {fileId} / Current Id = {id}");
            }

            var transactions = new List<DeployTransaction>();
            var actions = new List<string>();

            foreach (XmlNode node in root.ChildNodes)
            {
                if (NotEmptyNode(node, "sql"))
                {
                    var transaction = GetDeployTransactionFromNode(node);

                    if (transaction != null)
                    {
                        transactions.Add(transaction);
                    }
                }

                if (NotEmptyNode(node, "action"))
                {
                    var action = node.InnerText.Trim();

                    actions.Add(action);
                }
            }

            return new DeployFileInfo(id, transactions, actions);
        }

        private static bool NotEmptyNode(XmlNode node, string name)
        {
            return node.Name.ToLower() == name && (!node.InnerText.IsNullOrEmptyOrWhiteSpace() || node.ChildNodes.Count > 0);
        }

        private static DeployTransaction? GetDeployTransactionFromNode(XmlNode node)
        {
            if (node.ChildNodes.Count == 0 || node.FirstChild.NodeType == XmlNodeType.Text)
            {
                return new DeployTransaction(node.InnerText);
            }

            XmlNode? textNode = null;
            var checkList = new List<DeployTransactionCheck>();

            foreach (XmlNode innerNode in node.ChildNodes)
            {
                if (NotEmptyNode(innerNode, "text"))
                {
                    textNode = innerNode;

                    continue;
                }

                if (NotEmptyNode(innerNode, "check"))
                {
                    new DeployTransactionCheck(innerNode.InnerText)
                        .AddTo(checkList);
                }
            }

            if (textNode == null)
            {
                return null;
            }

            return new DeployTransaction(textNode.InnerText, checkList);
        }

        private static void ExecuteDeployFile(StartContext db, DeployFileInfo info)
        {
            var deploy = new ReadmeDeploy(info.Id, TimeHelper.GetCurrentDate());

            if (info.IsEmpty)
            {
                SaveDeploy(db, deploy);

                return;
            }

            var transactionsResult = true;

            if(info.Transactions.Count > 0)
            {
                transactionsResult = ExecuteTransactions(db, info.Transactions);
            }

            if(transactionsResult == false)
            {
                return;
            }

            var actionsResult = true;

            if (info.Actions.Count > 0)
            {
                actionsResult = ExecuteActions(info.Actions);
            }

            if(actionsResult == false)
            {
                return;
            }

            deploy.Timestamp = TimeHelper.GetCurrentDate();

            SaveDeploy(db, deploy);
        }

        private static void SaveDeploy(StartContext db, ReadmeDeploy deploy)
        {
            db.Add(deploy);
            db.SaveChanges();
        }

        private static bool ExecuteTransactions(StartContext db, IReadOnlyList<DeployTransaction> transactions)
        {
            var result = true;

            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var transaction in transactions)
                    {
                        foreach (var check in transaction.Checks)
                        {
                            var resultCheck = db.Database.SqlQueryRaw<bool>(check.Sql).ToList();
                            check.IsChecked = resultCheck.FirstOrDefault();
                        }

                        if (transaction.Check())
                        {
                            db.Database.ExecuteSqlRaw(transaction.Sql);
                        }
                    }

                    db.SaveChanges();
                    dbTransaction.Commit();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    dbTransaction.Rollback();

                    result = false;
                }
            }

            return result;
        }

        private static bool ExecuteActions(IReadOnlyList<string> actions)
        {
            foreach (var action in actions)
            {
                var success = Invoker.InvokeAction(action);

                if(success == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
