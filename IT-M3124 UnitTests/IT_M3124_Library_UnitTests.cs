using System.Net;

namespace IT_M3124_Library
{
    [TestClass]
    public class IT_M3124_Library_UnitTests
    {
        //Used for locking the port so we can create new connections on different ports
        private static Mutex mut = new Mutex();

        private IPAddress ip;
        private static int port = 4000;

        [TestInitialize]
        public void TestInitialize()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            ip = host.AddressList[0];
        }

        public IT_M3124_Emulator GetServer(int port)
        {
            Console.WriteLine("Creating server on port " + port);
            IT_M3124_Emulator server = new IT_M3124_Emulator(ip, port);
            Thread s = new Thread(server.StartServer);
            s.Start();
            return server;
        }
        public IT_M3124 GetController(int port)
        {
            IT_M3124 controller = new IT_M3124(ip, port);
            controller.SetRemote();
            return controller;
        }

        [TestMethod]
        public void GetCurrentValue()
        {
            mut.WaitOne();
            port += 1;

            IT_M3124_Emulator server = GetServer(port);
            IT_M3124 controller = GetController(port);
            double result = controller.GetCurrent();

            Assert.IsNotNull(result);

            mut.ReleaseMutex();
        }
        [TestMethod]
        public void SetCurrentValue()
        {
            mut.WaitOne();
            port += 1;

            IT_M3124_Emulator server = GetServer(port);
            IT_M3124 controller = GetController(port);

            double init = controller.GetCurrent();
            controller.SetCurrent(init * 2);
            double result = controller.GetCurrent();
            Assert.AreNotEqual(init, result);

            mut.ReleaseMutex();
        }
        [TestMethod]
        public void GetVoltageValue()
        {
            mut.WaitOne();
            port += 1;

            IT_M3124_Emulator server = GetServer(port);
            IT_M3124 controller = GetController(port);

            double result = controller.GetVoltage();

            Assert.IsNotNull(result);

            mut.ReleaseMutex();
        }
        [TestMethod]
        public void SetVoltageValue()
        {
            mut.WaitOne();
            port += 1;

            IT_M3124_Emulator server = GetServer(port);
            IT_M3124 controller = GetController(port);

            double init = controller.GetVoltage();
            controller.SetVoltage(init * 2);
            double result = controller.GetVoltage();
            Assert.AreNotEqual(init, result);

            mut.ReleaseMutex();
        }
        [TestMethod]
        public void GetOutputState()
        {
            mut.WaitOne();
            port += 1;

            IT_M3124_Emulator server = GetServer(port);
            IT_M3124 controller = GetController(port);

            bool result = controller.GetOutputState();

            Assert.IsNotNull(result);

            mut.ReleaseMutex();
        }
        [TestMethod]
        public void SetOutputState()
        {
            mut.WaitOne();
            port += 1;

            IT_M3124_Emulator server = GetServer(port);
            IT_M3124 controller = GetController(port);

            bool init = controller.GetOutputState();
            controller.SetOutputState(!init);
            bool result = controller.GetOutputState();
            Assert.AreNotEqual(init, result);

            mut.ReleaseMutex();
        }
    }
}