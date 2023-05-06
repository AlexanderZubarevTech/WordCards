using CardWords.Core.Commands;
using CardWords.Core.Contexts;

namespace CardWords.Core.ReadmeDeploy
{
    public class NeedDeployCommand : EntityCommand, INeedDeployCommand
    {
        public bool Execute()
        {
            var result = false;            

            using(var db = new StartContext())
            {
                var newFiles = DeployFilesHelper.GetNewFiles(db.Deploys);

                result = newFiles.Count > 0;
            }

            return result;
        }
    }
}
