using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TenK.InfoSparks.Common.Contracts
{
    public delegate void UseServiceDelegate<in T>(T proxy);

    public static class Service<T>
    {
        public static ChannelFactory<T> ChannelFactory = null;

         public static void Use(UseServiceDelegate<T> codeBlock, EndpointAddress endpointAddress = null)
        {
            if (ChannelFactory == null)
            {
                ChannelFactory = endpointAddress == null ? new ChannelFactory<T>(String.Empty) : new ChannelFactory<T>("",endpointAddress);
            }
        
             IClientChannel channel = (IClientChannel)ChannelFactory.CreateChannel();
            bool success = false;
            try
            {
                codeBlock((T)channel);
                channel.Close();
                success = true;
            }
            finally
            {
                if (!success)
                {
                    channel.Abort();
                }
            }
        }
    }
}
