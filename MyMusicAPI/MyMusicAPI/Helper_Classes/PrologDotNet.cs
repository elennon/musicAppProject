using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MyMusicAPI.Helper_Classes
{
    public class PrologDotNet
    {
        private void try1()
        {
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"the_PATH_to_boot32.prc");  // or boot64.prc
            //if (!PlEngine.IsInitialized)
            //{
            //    String[] param = { "-q" };  // suppressing informational and banner messages
            //    PlEngine.Initialize(param);
            //    PlQuery.PlCall("assert(father(martin, inka))");
            //    PlQuery.PlCall("assert(father(uwe, gloria))");
            //    PlQuery.PlCall("assert(father(uwe, melanie))");
            //    PlQuery.PlCall("assert(father(uwe, ayala))");
            //    using (var q = new PlQuery("father(P, C), atomic_list_concat([P,' is_father_of ',C], L)"))
            //    {
            //        foreach (PlQueryVariables v in q.SolutionVariables)
            //            Console.WriteLine(v["L"].ToString());

            //        Console.WriteLine("all children from uwe:");
            //        q.Variables["P"].Unify("uwe");
            //        foreach (PlQueryVariables v in q.SolutionVariables)
            //            Console.WriteLine(v["C"].ToString());
            //    }
            //    PlEngine.PlCleanup();
            //    Console.WriteLine("finshed!");
            //}
        }
    }
}