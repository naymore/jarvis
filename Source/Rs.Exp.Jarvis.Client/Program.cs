using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Rs.Exp.Jarvis.Core.Movement;
using Rs.Exp.Jarvis.Core.Navigation;
using Rs.Exp.Jarvis.Core.Poi;
using Rs.Exp.Jarvis.Navigation.Google;

namespace Rs.Exp.Jarvis.Client
{
    public class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static int Main(string[] args)
        {
            _logger.Info("Jarvis starting...");

            Thread.CurrentThread.CurrentCulture = new CultureInfo(1033);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(1033);

            // COMPOSITION ROOT
            INavigator navigator = new GoogleWalkingNavigator();
            IMoveStrategy moveStrategy = new WalkMoveStrategy();
            moveStrategy.SetMovementSpeed(25);

            IPoiProvider poiProvider = new FakePoiProvider();

            JarvisClient jarvis = new JarvisClient(navigator, moveStrategy, poiProvider);

            // START
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task jarvisRunner = Task.Run(() => jarvis.Run(cancellationTokenSource.Token), cancellationTokenSource.Token);

            // management thread
            Task.Run(() =>
                    {
                        while (true)
                        {
                            char key = Console.ReadKey(true).KeyChar;
                            switch (key)
                            {
                                case 'q':
                                    {
                                        // stop jarvis.
                                        cancellationTokenSource.Cancel();
                                        break;
                                    }
                            }
                        }
                    }, cancellationTokenSource.Token);

            try
            {
                jarvisRunner.Wait(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }

            _logger.Info("Jarvis shutdown complete.");
            return 0;
        }
    }
}