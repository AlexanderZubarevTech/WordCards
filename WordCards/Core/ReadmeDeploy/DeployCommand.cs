﻿using Microsoft.EntityFrameworkCore;
using WordCards.Core.Commands;
using WordCards.Core.Contexts;
using WordCards.Core.Helpers;
using WordCards.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace WordCards.Core.ReadmeDeploy
{
    public sealed partial class DeployCommand : EntityCommand, IDeployCommand
    {
        public void Execute()
        {
            using(var db = new StartContext())
            {
                var fileTransactions = GetFileTransactions(db.Deploys);

                if(fileTransactions.Count  > 0 )
                {
                    foreach(var transaction in fileTransactions)
                    {
                        ExecuteDeployFile(db, transaction.Key, transaction.Value);
                    }
                }                
            }
        }

        private static IReadOnlyDictionary<string, List<DeployTransaction>> GetFileTransactions(DbSet<ReadmeDeploy> deploys)
        {
            var newFiles = DeployFilesHelper.GetNewFiles(deploys);

            if(newFiles.Count == 0)
            {
                return DictionaryHelper.Empty<string, List<DeployTransaction>>();
            }

            var xmlFiles = GetXmlFiles(newFiles);

            return xmlFiles.ToDictionary(x => x.Key, x => GetSql(x.Key, x.Value));
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

        private static List<DeployTransaction> GetSql(string fileId, FileInfo file)
        {
            var doc = new XmlDocument();
            doc.Load(file.OpenText());

            var root = doc.DocumentElement;

            if (root == null)
            {
                return new List<DeployTransaction>();
            }

            var id = root.Attributes.GetNamedItem("id")?.Value;

            if (fileId != id)
            {
                throw new Exception($"Wrong File. File Id = {fileId} / Current Id = {id}");
            }

            var result = new List<DeployTransaction>(root.ChildNodes.Count);

            foreach (XmlNode node in root.ChildNodes)
            {
                if (NotEmptyNode(node, "sql"))
                {
                    var transaction = GetDeployTransactionFromNode(node);
                    
                    if(transaction != null)
                    {
                        result.Add(transaction);
                    }
                }
            }

            return result;
        }

        private static bool NotEmptyNode(XmlNode node, string name)
        {
            return node.Name.ToLower() == name && (!node.InnerText.IsNullOrEmptyOrWhiteSpace() || node.ChildNodes.Count > 0);
        }

        private static DeployTransaction? GetDeployTransactionFromNode(XmlNode node)
        {
            if(node.ChildNodes.Count == 0 || node.FirstChild.NodeType == XmlNodeType.Text)
            {
                return new DeployTransaction(node.InnerText);
            }

            XmlNode? textNode = null;
            var checkList = new List<DeployTransactionCheck>();

            foreach (XmlNode innerNode in node.ChildNodes)
            {
                if(NotEmptyNode(innerNode, "text"))
                {
                    textNode = innerNode;

                    continue;
                }

                if(NotEmptyNode(innerNode, "check"))
                {
                    new DeployTransactionCheck(innerNode.InnerText)
                        .AddTo(checkList);
                }
            }

            if(textNode == null)
            {
                return null;
            }

            return new DeployTransaction(textNode.InnerText, checkList);
        }
                
        private static void ExecuteDeployFile(StartContext db, string id, List<DeployTransaction> transactions)
        {
            var deploy = new ReadmeDeploy(id, TimeHelper.GetCurrentDate());

            if(transactions.Count == 0)
            {
                db.Add(deploy);
                db.SaveChanges();

                return;
            }

            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var transaction in transactions)
                    {
                        foreach(var check in transaction.Checks)
                        {
                            var resultCheck = db.Database.SqlQueryRaw<bool>(check.Sql).ToList();
                            check.IsChecked = resultCheck.FirstOrDefault();
                        }

                        if(transaction.Check())
                        {
                            db.Database.ExecuteSqlRaw(transaction.Sql);
                        }
                    }                    

                    deploy.Timestamp = TimeHelper.GetCurrentDate();

                    db.Add(deploy);
                    db.SaveChanges();

                    dbTransaction.Commit();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());                    

                    dbTransaction.Rollback();
                }
            }           
        }
    }
}
