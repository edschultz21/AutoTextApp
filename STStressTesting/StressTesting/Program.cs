using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace STStressTesting
{
    /*
        DevServer: crmls.devstats.showingtime.com
        DevPages: Cmue-Rl6,Cmuc-29j,CmuG-Ms6
        LocalServer: localhost:3900
        LocalPages: t7-IwT,tb-CEa,tZ-2L3,tf-Qaq,tY-Xdn,tF-EPX,tR-KJl,tw-IKR,tO-ysq,tA-0Zh,t3-XNf,tX-i1B,tz-LuJ,tD-CL2,tL-eMJ,tM-qwR,ti-CSk
        LocalPagesPDF: tv-L2W,te-HPl,t9-ohd,tn-vlH,tc-Rqg,tG-Kzg,dB-d9E,dd-tBl,dm-bvN,d2-EBl,d7-ZiW
   */

    class Program
    {
        static void Main(string[] args)
        {
            var settings = Properties.Settings.Default;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            new SemaphoreSlimImp().Run(settings);

            //if (settings.startThread == 0)
            //{
            //    new TaskPoolTest().Run(settings);
            //}
            //else
            //{
            //    new ThreadTest().Run(settings);
            //}

            Console.WriteLine();

            sw.Stop();
            Console.WriteLine("Total time: " + sw.Elapsed.ToString(@"mm\:ss\.ff"));

            Console.WriteLine();
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}


