﻿using MQTTnet.Core.Diagnostics;
using MQTTnet.Core.Packets;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MQTTnet.Core.Client
{
    public class MqttClientQueuedPersistentMessagesManager
    {
        private readonly IList<MqttApplicationMessage> _persistedMessages = new List<MqttApplicationMessage>();
        private readonly MqttClientManagedOptions _options;

        public MqttClientQueuedPersistentMessagesManager(MqttClientManagedOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task LoadMessagesAsync()
        {
            try
            {
                var persistentMessages = await _options.Storage.LoadQueuedMessagesAsync();
                lock (_persistedMessages)
                {
                    _persistedMessages.Clear();
                    foreach (var persistentMessage in persistentMessages)
                    {
                        _persistedMessages.Add(persistentMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                MqttNetTrace.Error(nameof(MqttClientQueuedPersistentMessagesManager), exception, "Unhandled exception while loading persistent messages.");
            }
        }

        public async Task SaveMessageAsync(MqttApplicationMessage applicationMessage)
        {
            if (applicationMessage != null)
            {
                lock (_persistedMessages)
                {
                    _persistedMessages.Add(applicationMessage);
                }
            }
            try
            {               
                if (_options.Storage != null)
                {
                    await _options.Storage.SaveQueuedMessagesAsync(_persistedMessages);
                }
            }
            catch (Exception exception)
            {
                MqttNetTrace.Error(nameof(MqttClientQueuedPersistentMessagesManager), exception, "Unhandled exception while saving persistent messages.");
            }
        }

        public List<MqttApplicationMessage> GetMessages()
        {
            var persistedMessages = new List<MqttApplicationMessage>();
            lock (_persistedMessages)
            {
                foreach (var persistedMessage in _persistedMessages)
                {
                    persistedMessages.Add(persistedMessage);
                }
            }

            return persistedMessages;
        }

        public async Task Remove(MqttApplicationMessage message)
        {
            lock (_persistedMessages)
                _persistedMessages.Remove(message);

            await SaveMessageAsync(null);
        }
    }
}