using System;

namespace BackgroundProcessPoc
{
    public class BackgroundJobActivator : Hangfire.JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public BackgroundJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
