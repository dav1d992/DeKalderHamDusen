﻿using DeKalderHamDusen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeKalderHamDusen
{
    public class GennemfoerVask
    {
        private enum VaskeState
        {
            Ledig,
            KlarTilVask,
            Vask,
            Udkoersel
        };

        private VaskeState _state;
        private PortStyring _port;
        private SkilteStyring _skilte;
        private UdførVaskeSekvens _vaskeSekvens;
        private UserInterface _ui;
        private AfstandsSensor _sensor;

        public GennemfoerVask(PortStyring port, SkilteStyring skilte, UdførVaskeSekvens vaskeSekvens, UserInterface ui, AfstandsSensor sensor)
        {
            _port = port;
            _skilte = skilte;
            _vaskeSekvens = vaskeSekvens;
            _state = VaskeState.Ledig;
            _ui = ui;
            _sensor = sensor;

            vaskeSekvens.VaskAfsluttet += HandleVaskAfsluttet;
            _ui.ValgAfVaskEvent += HandleValgAfVask;
            _sensor.AfstandEvent += HandleAfstandEvent;
        }

        private void HandleValgAfVask(object source, ValgAfVaskEventArgs e)
        {
            switch (_state)
            {
                case VaskeState.KlarTilVask:
                    _port.LukPort();
                    _vaskeSekvens.Start(e.VaskeType);
                    _state = VaskeState.Vask;
                    break;
            }
        }

        private void HandleVaskAfsluttet(object source, VaskAfsluttetEventArgs e)
        {
            switch (_state)
            {
                case VaskeState.Vask:
                    _port.AabenPort();
                    _skilte.KoerTilbage();
                    _state = VaskeState.Udkoersel;
                    break;
            }
        }

        private void HandleAfstandEvent(object source, AfstandEventArgs e)
        {
            if (e.Afstand < 150)
            {
                _skilte.KoerTilbage();
                _state = VaskeState.KlarTilVask;
            }
            else if (e.Afstand >= 150 && e.Afstand <= 200 && _state != VaskeState.Vask)
            {
                _skilte.Stop();
                _state = VaskeState.KlarTilVask;
            }
            else if (e.Afstand > 200 && e.Afstand <= 999 && _state != VaskeState.Vask)
            {
                _skilte.KoerFrem();
                _state = VaskeState.Ledig;
            }
            else if (e.Afstand <= 999 && _state == VaskeState.Vask)
            {
                _skilte.KoerTilbage();
                _state = VaskeState.Udkoersel;
            }
            else if (e.Afstand >= 1000)
            {
                _skilte.KoerFrem();
                _state = VaskeState.Ledig;
            }
        }
    }

    public class PortStyring
    {
        public void LukPort()
        {
            Console.WriteLine("Porten er lukket.");
            // Tilføj rigtig logik her
        }

        public void AabenPort()
        {
            Console.WriteLine("Porten er åbnet.");
            // Tilføj rigtig logik her
        }
    }

    public class UdførVaskeSekvens
    {
        public event EventHandler<VaskAfsluttetEventArgs> VaskAfsluttet;

        public void Start(int vaskeType)
        {
            Console.WriteLine($"Starter vasketype {vaskeType}...");
            Thread.Sleep(5000);  // Simulerer tid det tager at vaske bilen
            VaskAfsluttet?.Invoke(this, new VaskAfsluttetEventArgs());
        }
    }

    public class ValgAfVaskEventArgs : EventArgs
    {
        public int VaskeType { get; set; }
    }

    public class VaskAfsluttetEventArgs : EventArgs
    {
    }

    public class UserInterface
    {
        public event EventHandler<ValgAfVaskEventArgs> ValgAfVaskEvent;

        public void VaelgVask(VaskType vaskType)
        {
            ValgAfVaskEvent?.Invoke(this, new ValgAfVaskEventArgs { VaskeType = (int)vaskType });
        }
    }

    public enum VaskType
    {
        Standard,
        Premium
    }

    public class AfstandsSensor
    {
        public event EventHandler<AfstandEventArgs> AfstandEvent;

        private int _afstand = 1000;

        public void SetAfstand(int afstand)
        {
            if (_afstand != afstand)
            {
                _afstand = afstand;
                AfstandEvent?.Invoke(this, new AfstandEventArgs { Afstand = _afstand });
            }
        }
    }

    public class AfstandEventArgs : EventArgs
    {
        public int Afstand { get; set; }
    }

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