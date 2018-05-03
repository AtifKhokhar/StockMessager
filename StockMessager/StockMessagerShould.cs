﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace StockMessager
{
    [TestFixture]
    public class StockMessagerShould
    {
        private MessagingFactory factory;
        private MessageSender sender;

        [SetUp]
        public void SetUp()
        {
             factory = MessagingFactory.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["StockEventServiceBus"].ToString());
             sender = factory.CreateMessageSender("stocklevels");
        }


        [Test]
        public void PublishMessageOfStockItem()
        {
            //arrange
            var stockItem = new StockItem("ABC",1,"FCLondon","DHL");
            var stockMessagerService = new StockMessagerService(sender);
            var reciever = factory.CreateMessageReceiver("stocklevels/subscriptions/GBWarehouseStockLevels");
            //act
             stockMessagerService.SendMessageAsync(stockItem).Wait();
            var message = reciever.Receive();
            //assert
            Assert.That(message.GetBody<StockItem>().Sku.Equals("ABC"));
        }

    }
}