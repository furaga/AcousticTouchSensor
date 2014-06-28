using AcousticTouchSensor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///AcousticSensorTest のテスト クラスです。すべての
    ///AcousticSensorTest 単体テストをここに含めます
    ///</summary>
    [TestClass()]
    public class AcousticSensorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        // 
        //テストを作成するときに、次の追加属性を使用することができます:
        //
        //クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //各テストを実行する前にコードを実行するには、TestInitialize を使用
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //各テストを実行した後にコードを実行するには、TestCleanup を使用
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///AddBytes のテスト
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AcousticTouchSensor.exe")]
        public void AddBytesTest()
        {
            PrivateObject param0 = null; // TODO: 適切な値に初期化してください
            AcousticSensor_Accessor target = new AcousticSensor_Accessor(param0); // TODO: 適切な値に初期化してください
            byte[] bytes = null; // TODO: 適切な値に初期化してください
            int count = 0; // TODO: 適切な値に初期化してください
            target.AddBytes(bytes, count);
            Assert.Inconclusive("値を返さないメソッドは確認できません。");
        }
    }
}
