using NUnit.Framework;
using Moq;
using DeKalderHamDusen;
using DeKalderHamDusen.Interfaces;

namespace DeKalderHamDusenTests
{
    public class GennemfoerVaskTests
    {
        private Mock<IPortStyring> _mockPort;
        private Mock<ISkilteStyring> _mockSkilte;
        private Mock<IUserInterface> _mockUi;
        private Mock<IAfstandsSensor> _mockSensor;
        private GennemfoerVask _gennemfoerVask;
        private Mock<IUdførVaskeSekvens> _mockVaskeSekvens;

        [SetUp]
        public void Setup()
        {
            _mockPort = new Mock<IPortStyring>();
            _mockSkilte = new Mock<ISkilteStyring>();
            _mockVaskeSekvens = new Mock<IUdførVaskeSekvens>();
            _mockUi = new Mock<IUserInterface>();
            _mockSensor = new Mock<IAfstandsSensor>();
            _gennemfoerVask = new GennemfoerVask(_mockPort.Object, _mockSkilte.Object, _mockVaskeSekvens.Object, _mockUi.Object, _mockSensor.Object);
        }

        [Test]
        public void Test_HandleValgAfVask_CallsStartAndLukPort_WhenStateIsKlarTilVask()
        {
            // Arrange
            // Set the state to KlarTilVask by handling an AfstandEvent with Afstand between 150 and 200
            _gennemfoerVask.HandleAfstandEvent(_mockSensor.Object, new AfstandEventArgs { Afstand = 175 });

            // Act
            _gennemfoerVask.HandleValgAfVask(_mockUi.Object, new ValgAfVaskEventArgs { VaskeType = (int)VaskType.Standard });

            // Assert
            _mockVaskeSekvens.Verify(x => x.Start((int)VaskType.Standard), Times.Once);
            _mockPort.Verify(x => x.LukPort(), Times.Once);
        }


        [Test]
        public void Test_HandleVaskAfsluttet_CallsAabenPortAndKoerTilbage_WhenStateIsVask()
        {
            // Arrange
            _mockSensor.Raise(m => m.AfstandEvent += null, new AfstandEventArgs { Afstand = 100 });
            _mockUi.Raise(m => m.ValgAfVaskEvent += null, new ValgAfVaskEventArgs { VaskeType = (int)VaskType.Standard });

            // Reset mock verification count
            _mockSkilte.Reset();

            // Act: now we can test HandleVaskAfsluttet
            _gennemfoerVask.HandleVaskAfsluttet(_mockVaskeSekvens.Object, new VaskAfsluttetEventArgs());

            // Assert
            _mockPort.Verify(x => x.AabenPort(), Times.Once);
            _mockSkilte.Verify(x => x.KoerTilbage(), Times.Once);
        }

    }
}
