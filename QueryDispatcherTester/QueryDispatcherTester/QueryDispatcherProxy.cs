using System;
using System.ServiceModel;
using System.Threading;
using TenK.InfoSparks.Common.Contracts.QueryDispatcherContract;

namespace QueryDispatcherTester
{
    public delegate void UseServiceDelegate<in TQueryDispatcherClient>(TQueryDispatcherClient proxy);

    public class DispatcherProxy
    {
        private IDispatcherExecuteContract _proxy = null;
        private readonly string _dispatcherKey = "";
        private readonly string _queryCacheKey = "";
        private readonly int _serverRetryLimit;
        private readonly InstanceContext _context;
        private readonly EndpointAddress _endPoint;
        // Picks up configuration from the config file.
        private  DuplexChannelFactory<IDispatcherExecuteContract> _factory;
        private readonly IDispatcherExecuteCallbackContract _callbackContract;


        internal DispatcherProxy(string queryCacheKey, string dispatcherKey, string serviceAddress, int serverRetryLimit, IDispatcherExecuteCallbackContract callback)
        {
            if (String.IsNullOrWhiteSpace(serviceAddress))
            {
                throw new ArgumentNullException("serviceAddress","service address must always be filled in.");
            }
            _callbackContract = callback;
            _endPoint = new EndpointAddress(serviceAddress);            
            _dispatcherKey = dispatcherKey;
            _queryCacheKey = queryCacheKey;
            _serverRetryLimit = serverRetryLimit;
        }

        internal void Execute(UseServiceDelegate<IDispatcherExecuteContract> codeBlock)
        {
            bool success = false;
            int retries = 0;
            do
            {
                CreateChannelIfNeeded();
                try
                {
                    codeBlock(_proxy);
                    success = true;
                }
                catch (Exception e)
                {
                    // Log error
                    Console.WriteLine("Error executing the action. Dispatcher=" + _dispatcherKey + ", Message: " + e.Message, e);
                    if (retries++ > _serverRetryLimit)
                    {
                        throw;
                    }
                    Thread.Sleep(1000);
                }
                finally
                {

                }
            } while (!success);
        }

        private void CreateChannelIfNeeded()
        {
            // We need to protect this with a lock, so its only initialized once
            lock (_dispatcherKey)
            {
                //ICommunicationObject channel = (ICommunicationObject) _proxy;
                IClientChannel channel = ((IClientChannel)_proxy);
                if (_proxy == null ||
                    !(channel.State == CommunicationState.Opened || channel.State == CommunicationState.Opening))
                {
                    //Console.WriteLine("Need to create new channel with {1}. {0}", channel == null ? "Proxy is null" : "Current State = " + channel.State,_dispatcherKey);
                    CloseChannel();
                    CreateNewChannel();
                }
            }
        }

        /* These methods only used by CreateChannelIfNeeded */

        private void CreateNewChannel()
        {
            if (_factory == null)
            {
                _factory = new DuplexChannelFactory<IDispatcherExecuteContract>(new InstanceContext(_callbackContract), "NetTcpBinding_IDispatcherExecuteContract", _endPoint);                
            }
            _proxy = _factory.CreateChannel();
            _proxy.RegisterService(_queryCacheKey);
            //Console.WriteLine("Created new channel with dispatcher {0}",_dispatcherKey);
        }

        private void CloseChannel()
        {
            if (_proxy != null)
            {
                ICommunicationObject channel = (ICommunicationObject) _proxy;
                if (channel.State == CommunicationState.Faulted)
                {
                    channel.Abort();
                    _factory.Abort();
                    _factory = null;
                    Console.WriteLine("Channel Aborted. Dispatcher={0}", _dispatcherKey);
                }
                else
                {
                    channel.Close();
                    _factory.Close();
                    _factory = null;
                    Console.WriteLine("Channel Closed. Dispatcher={0}", _dispatcherKey);
                }
                ((IDisposable)channel).Dispose();
                _proxy = null;
            }
        }
    }
}
