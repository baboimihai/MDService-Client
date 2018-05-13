using MDService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;

using System.Web.Http;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiClasses.Controllers
{

    public class DMicroserviceController : ApiController
    {

        [HttpPost]
        public object Get()
        {
            var input = Request.Content.ReadAsStringAsync().Result;

            var assembly = Assembly.LoadFrom("process/ClassLibraryMath.dll");
            var type = assembly.GetType("ClassLibraryMath.LibraryMath"); //);
            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod("RunFromMicroservice");

                if (methodInfo != null)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    object classInstance = Activator.CreateInstance(type, null);
                    object[] parametersArray = new object[] { input };
                    var result = methodInfo.Invoke(classInstance, parametersArray);
                    return result;
                }
            }
            return "hello from class library";
        }
        [HttpPost]
        public void RestartServer()
        {
            Server.serviceTask.Restart();

        }
        [HttpPost]
        public string InstallService(InstallServiceDto input)
        {

            
            // Start program
            try
            {
                var byteArrayDes = JsonConvert.DeserializeObject<List<int>>(input.ByteArray);
                var newArray = new byte[byteArrayDes.Count];
                int i = 0;
                byteArrayDes.ForEach(x =>
                {
                    newArray[i++] = (byte)x;
                });
                if (File.Exists("install.zip"))
                    File.Delete("install.zip");
                if (Directory.Exists("process"))
                    clearFolder("process");
                File.WriteAllBytes("install.zip", newArray);
                using (Ionic.Zip.ZipFile zip1 = Ionic.Zip.ZipFile.Read("install.zip"))
                {
                    zip1.ExtractAll("process",
                    Ionic.Zip.ExtractExistingFileAction.DoNotOverwrite);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }


            return "OK";
        }
        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
    }
    public class InstallServiceDto
    {
        public string ByteArray { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }
    }


    public class RunServerTash : IDisposable
    {
        public void Dispose()
        {

        }

        public object Run(string input)
        {
            var assembly = Assembly.LoadFrom("process/ClassLibraryMath.dll");

            var type = assembly.GetType("ClassLibraryMath.LibraryMath");
            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod("RunFromMicroservice");

                if (methodInfo != null)
                {
                    object result = null;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    object classInstance = Activator.CreateInstance(type, null);
                    object[] parametersArray = new object[] { input };
                    result = methodInfo.Invoke(classInstance, parametersArray);
                    return result;
                }
            }
            return "hello from class library";
        }
    }
}