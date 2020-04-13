using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace GarageDoorService
{
    using System.Threading.Tasks;
    using Services;


    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //

            _deferral = taskInstance.GetDeferral();

            var service = new GarageDoorService();

            // fire off task to initialize the garage door service
            Task.Run(async () => await service.Initialize());

            WebServer.WebServer.StartServer(service);
            
        }
    }
}
