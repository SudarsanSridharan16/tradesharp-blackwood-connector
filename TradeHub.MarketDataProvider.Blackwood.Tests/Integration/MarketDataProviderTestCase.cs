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


﻿using System;
using System.Threading;
using NUnit.Framework;
using Spring.Context.Support;
using TradeHub.Common.Core.Constants;
using TradeHub.Common.Core.DomainModels;
using TradeHub.Common.Core.ValueObjects.MarketData;
using TradeHub.MarketDataProvider.Blackwood.Provider;

namespace TradeHub.MarketDataProvider.Blackwood.Tests.Integration
{
    [TestFixture]
    public class MarketDataProviderTestCase
    {
        private BlackwoodMarketDataProvider _marketDataProvider;
        [SetUp]
        public void SetUp()
        {
            _marketDataProvider = ContextRegistry.GetContext()["BlackwoodMarketDataProvider"] as BlackwoodMarketDataProvider;
        }

        [Test]
        [Category("Integration")]
        public void ConnectMarketDataProviderTestCase()
        {
            bool isConnected = false;
            var manualLogonEvent = new ManualResetEvent(false);

            _marketDataProvider.LogonArrived +=
                    delegate(string obj)
                        {
                            isConnected = true;
                            manualLogonEvent.Set();
                        };

            _marketDataProvider.Start();
            manualLogonEvent.WaitOne(30000, false);

            Assert.AreEqual(true, isConnected);
        }

        [Test]
        [Category("Integration")]
        public void DisconnectMarketDataProviderTestCase()
        {
            bool isConnected = false;
            var manualLogonEvent = new ManualResetEvent(false);
            _marketDataProvider.LogonArrived +=
                    delegate(string obj)
                    {
                        isConnected = true;
                        _marketDataProvider.Stop();
                        manualLogonEvent.Set();
                    };

            bool isDisconnected = false;
            var manualLogoutEvent = new ManualResetEvent(false);
            _marketDataProvider.LogoutArrived +=
                    delegate(string obj)
                    {
                        isDisconnected = true;
                        manualLogoutEvent.Set();
                    };

            _marketDataProvider.Start();
            manualLogonEvent.WaitOne(30000, false);
            manualLogoutEvent.WaitOne(30000, false);

            Assert.AreEqual(true, isConnected, "Connected");
            Assert.AreEqual(true, isDisconnected, "Disconnected");
        }

        [Test]
        [Category("Integration")]
        public void SubscribeMarketDataProviderTestCase()
        {
            bool isConnected = false;
            bool tickArrived = false;

            var manualLogonEvent = new ManualResetEvent(false);
            var manualTickEvent = new ManualResetEvent(false);

            _marketDataProvider.LogonArrived +=
                    delegate(string obj)
                    {
                        isConnected = true;
                        _marketDataProvider.SubscribeTickData(new Subscribe(){Security = new Security(){Symbol = "AGQ"}});
                        manualLogonEvent.Set();
                    };

            _marketDataProvider.TickArrived +=
                    delegate(Tick obj)
                    {
                        tickArrived = true;
                        _marketDataProvider.Stop();
                        manualTickEvent.Set();
                    };

            _marketDataProvider.Start();
            manualLogonEvent.WaitOne(30000, false);
            manualTickEvent.WaitOne(30000, false);
            Assert.AreEqual(true, isConnected,"Is Market Data Provider connected");
            Assert.AreEqual(true, tickArrived, "Tick arrived");
        }

        [Test]
        [Category("Integration")]
        public void RequestDailyHistoricalDataTestCase()
        {
            bool logonReceived = false;
            bool dataReceived = false;

            var logonManualResetEvent = new ManualResetEvent(false);
            var dataManualResetEvent = new ManualResetEvent(false);

            var dataRequestMessage = new HistoricDataRequest() { Security = new Security() { Symbol = "MSFT" } };
            dataRequestMessage.Id = "1";
            dataRequestMessage.BarType = BarType.DAILY;
            dataRequestMessage.Interval = 1;
            dataRequestMessage.StartTime = new DateTime(2015, 7, 15);
            dataRequestMessage.EndTime = new DateTime(2015, 7, 26);

            _marketDataProvider.LogonArrived += delegate (string providerName)
            {
                logonReceived = true;
                logonManualResetEvent.Set();

                _marketDataProvider.HistoricBarDataRequest(dataRequestMessage);
            };

            _marketDataProvider.HistoricBarDataArrived += delegate (HistoricBarData data)
            {
                dataReceived = true;
                dataManualResetEvent.Set();
            };

            _marketDataProvider.Start();

            logonManualResetEvent.WaitOne(10000, false);
            dataManualResetEvent.WaitOne(160000, false);

            Assert.AreEqual(true, logonReceived, "Logon Received");
            Assert.AreEqual(true, dataReceived, "Data Received");
        }

        [Test]
        [Category("Integration")]
        public void RequestIntradayHistoricalDataTestCase()
        {
            bool logonReceived = false;
            bool dataReceived = false;

            var logonManualResetEvent = new ManualResetEvent(false);
            var dataManualResetEvent = new ManualResetEvent(false);

            var dataRequestMessage = new HistoricDataRequest() { Security = new Security() { Symbol = "MSFT" } };
            dataRequestMessage.Id = "1";
            dataRequestMessage.BarType = BarType.INTRADAY;
            dataRequestMessage.Interval = 1;
            dataRequestMessage.StartTime = new DateTime(2015, 7, 15);
            dataRequestMessage.EndTime = new DateTime(2015, 7, 26);

            _marketDataProvider.LogonArrived += delegate (string providerName)
            {
                logonReceived = true;
                logonManualResetEvent.Set();

                _marketDataProvider.HistoricBarDataRequest(dataRequestMessage);
            };

            _marketDataProvider.HistoricBarDataArrived += delegate (HistoricBarData data)
            {
                dataReceived = true;
                dataManualResetEvent.Set();
            };

            _marketDataProvider.Start();

            logonManualResetEvent.WaitOne(10000, false);
            dataManualResetEvent.WaitOne(160000, false);

            Assert.AreEqual(true, logonReceived, "Logon Received");
            Assert.AreEqual(true, dataReceived, "Data Received");
        }

        [Test]
        [Category("Integration")]
        public void RequestMonthlyHistoricalDataTestCase()
        {
            bool logonReceived = false;
            bool dataReceived = false;

            var logonManualResetEvent = new ManualResetEvent(false);
            var dataManualResetEvent = new ManualResetEvent(false);

            var dataRequestMessage = new HistoricDataRequest() { Security = new Security() { Symbol = "MSFT" } };
            dataRequestMessage.Id = "1";
            dataRequestMessage.BarType = BarType.MONTHLY;
            dataRequestMessage.Interval = 1;
            dataRequestMessage.StartTime = new DateTime(2015, 7, 15);
            dataRequestMessage.EndTime = new DateTime(2015, 7, 26);

            _marketDataProvider.LogonArrived += delegate (string providerName)
            {
                logonReceived = true;
                logonManualResetEvent.Set();

                _marketDataProvider.HistoricBarDataRequest(dataRequestMessage);
            };

            _marketDataProvider.HistoricBarDataArrived += delegate (HistoricBarData data)
            {
                dataReceived = true;
                dataManualResetEvent.Set();
            };

            _marketDataProvider.Start();

            logonManualResetEvent.WaitOne(10000, false);
            dataManualResetEvent.WaitOne(160000, false);

            Assert.AreEqual(true, logonReceived, "Logon Received");
            Assert.AreEqual(true, dataReceived, "Data Received");
        }
    }
}
