using System;
using System.Threading;
using TenK.InfoSparks.Common.AnalysisServices;

namespace QueryDispatcherTester
{
    public class DispatcherResponseWaiter : IDisposable
    {
        private ManualResetEventSlim _finishedLock;
        private MDXQueryResult _result;
        private bool _errorFlag;
        private string _message;
        private readonly int _responseTimeout;

        public DispatcherResponseWaiter(int responseTimeout)
        {
            _responseTimeout = responseTimeout;
            _errorFlag = false;
            _finishedLock = new ManualResetEventSlim();
        }

        public MDXQueryResult WaitForAndGetResponse()
        {
            if (_finishedLock.Wait(_responseTimeout))
            {
                if (_errorFlag)
                {
                    throw new Exception("Error occurred in excuting query: " + _message);
                }
                else {
                    return _result;
                }
            }
            else
            {
                throw new Exception("ERROR: Dispatcher query timed out.");
            }
        }

        public void SetResultAndSignalFinished(MDXQueryResult result)
        {
            _result = result;
            _finishedLock.Set();
        }

        public void SetAndSignalError(string message)
        {
            _errorFlag = true;
            _message = message;
            _finishedLock.Set();
        }

        public void Dispose()
        {
            _finishedLock.Dispose();
        }
    }
}
