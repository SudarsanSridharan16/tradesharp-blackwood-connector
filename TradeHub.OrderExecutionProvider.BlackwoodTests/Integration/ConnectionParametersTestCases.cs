/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* TradeSharp is a C# based data feed and broker neutral Algorithmic 
* Trading Platform that lets trading firms or individuals automate 
* any rules based trading strategies in stocks, forex and ETFs. 
* TradeSharp allows users to connect to providers like Tradier Brokerage, 
* IQFeed, FXCM, Blackwood, Forexware, Integral, HotSpot, Currenex, 
* Interactive Brokers and more. 
* Key features: Place and Manage Orders, Risk Management, 
* Generate Customized Reports etc 
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using NUnit.Framework;
using Spring.Context.Support;
using TradeHub.OrderExecutionProvider.Blackwood.Provider;
using TradeHub.OrderExecutionProvider.Blackwood.Utility;
using TradeHub.OrderExecutionProvider.Blackwood.ValueObjects;

namespace TradeHub.OrderExecutionProvider.BlackwoodTests.Integration
{
    [TestFixture]
    public class ConnectionParametersTestCases
    {
        private ConnectionParametersLoader _parametersLoader;
        private ConnectionParameters _parameters;

        [SetUp]
        public void SetUp()
        {
            _parametersLoader = ContextRegistry.GetContext()["BWConnectionParametersLoader"] as ConnectionParametersLoader;
            if (_parametersLoader != null) _parameters = _parametersLoader.Parameters;
        }

        [Test]
        [Category("Integration")]
        public void ReadParametersTestCase()
        {
            Assert.AreEqual("35TEST", _parameters.UserName);
            Assert.AreEqual("123456", _parameters.Password);
            Assert.AreEqual("72.5.42.156", _parameters.Ip);
            Assert.AreEqual(5000, _parameters.Port);
            Assert.AreEqual(5300, _parameters.ClientPort);
        }
    }
}
