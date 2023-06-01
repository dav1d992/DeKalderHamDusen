using NUnit.Framework;
using Moq;
using DeKalderHamDusen;

namespace DeKalderHamDusenTests
{
    public class GennemfoerVaskTests
    {
        private Mock<IPortStyring> _mockPort;
        private Mock<SkilteStyring> _mockSkilte;
        private Mock<UserInterface> _mockUi;
        private Mock<AfstandsSensor> _mockSensor;
        private GennemfoerVask _gennemfoerVask;
        private Mock<IUdf�rVaskeSekvens> _mockVaskeSekvens;

        [SetUp]
        public void Setup()
        {
            _mockPort = new Mock<IPortStyring>();
            _mockSkilte = new Mock<SkilteStyring>();
            _mockVaskeSekvens = new Mock<IUdf�rVaskeSekvens>();
            _mockUi = new Mock<UserInterface>();
            _mockSensor = new Mock<AfstandsSensor>();
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
            _gennemfoerVask.HandleVaskAfsluttet(_mockVaskeSekvens.Object, new VaskAfsluttetEventArgs());

            // Assert
            _mockPort.Verify(x => x.AabenPort(), Times.Once);
            _mockSkilte.Verify(x => x.KoerTilbage(), Times.Once);
        }

        // Write more tests for HandleAfstandEvent method with different inputs and expected behaviour.
    }
}
