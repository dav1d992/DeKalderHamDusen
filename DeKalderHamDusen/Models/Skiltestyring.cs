using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeKalderHamDusen.Models
{
    public class SkilteStyring
    {
        private SkiltStatus _skiltStatus;

        public SkilteStyring()
        {
            _skiltStatus = SkiltStatus.KoerFrem;
        }

        public void KoerFrem()
        {
            _skiltStatus = SkiltStatus.KoerFrem;
            VisSkiltStatus();
        }

        public void Stop()
        {
            _skiltStatus = SkiltStatus.Stop;
            VisSkiltStatus();
        }

        public void KoerTilbage()
        {
            _skiltStatus = SkiltStatus.KoerTilbage;
            VisSkiltStatus();
        }

        private void VisSkiltStatus()
        {
            Console.WriteLine($"Skiltstatus: {_skiltStatus}");
        }

        private enum SkiltStatus
        {
            KoerFrem,
            Stop,
            KoerTilbage
        }
    }

}
