using MDService;
using MDService.UI;
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
        private static Assembly assembly;
        private static Type type;
        private static MethodInfo methodInfo;

        public string Test()
        {
            return "Hi!";
        }

        [HttpPost]
        public object Get()
        {
            if (Program.serviceTask.IsRunning)
            {
                var input = SessionIdentity.ConnectionSecurity.Decrypt(Request.Content.ReadAsStringAsync().Result);

                if (methodInfo != null)
                {
                    try{
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        object classInstance = Activator.CreateInstance(type, null);
                        object[] parametersArray = new object[] { (string)input, (byte[])SessionIdentity.Key.ToArray(), String.Copy(SessionIdentity.Token) };
                        var result = (string)methodInfo.Invoke(classInstance, parametersArray);
                        return SessionIdentity.ConnectionSecurity.Encrypt(result);
                    }catch(Exception e)
                    {
                        return e;
                    }

                }
            }
            return string.Empty;
        }
        [HttpPost]
        public object ExtendedService()
        {
            if (Program.serviceTask.IsRunning)
            {
                var input = SessionIdentity.ConnectionSecurity.Decrypt(Request.Content.ReadAsStringAsync().Result);
                var inputObject = JsonConvert.DeserializeObject<ExtendedMicroservice>(input);
                if (methodInfo != null)
                {
                    try
                    {
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        object classInstance = Activator.CreateInstance(type, null);
                        object[] parametersArray = new object[] { (string)inputObject.ContentJson, (byte[])SessionIdentity.Key.ToArray(), String.Copy(SessionIdentity.Token) };
                        var result = (string)methodInfo.Invoke(classInstance, parametersArray);
                        return SessionIdentity.ConnectionSecurity.Encrypt(result, inputObject.TemporaryPassword);
                    }
                    catch (Exception e)
                    {
                        return e;
                    }

                }
            }
            return string.Empty;
        }
        [HttpPost]
        public void RestartServer()
        {
            Program.serviceTask.Restart();


        }
        [HttpPost]
        public string CheckClient()
        {
            return SessionIdentity.Token;
        }
        [HttpPost]
        public string InstallService()
        {
            var input = JsonConvert.DeserializeObject<InstallServiceDto>(SessionIdentity.ConnectionSecurity.Decrypt(Request.Content.ReadAsStringAsync().Result));
            // Start program
            try
            {
                var byteArrayDes = JsonConvert.DeserializeObject<List<int>>(input.ByteArray);
                SessionIdentity.Namespace = input.Namespace;
                SessionIdentity.ClassName = input.ClassName;
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
                    Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                }
                assembly = Assembly.LoadFrom("process/" + input.Namespace + ".dll");
                type = assembly.GetType(input.Namespace + "." + input.ClassName);
                if (type != null)
                {
                    methodInfo = type.GetMethod("RunFromMicroservice");
                    return "Ok";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }


            return "No";
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
    public class ExtendedMicroservice
    {
        public string ContentJson { get; set; }
        public byte[] TemporaryPassword { get; set; }
        public string SThis { get; set; }

    }

}