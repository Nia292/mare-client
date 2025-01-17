﻿using MareSynchronos.Utils;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MareSynchronos.Mediator;

public class MareMediator : IDisposable
{
    private class SubscriberAction
    {
        public IMediatorSubscriber Subscriber { get; }
        public Action<IMessage> Action { get; }

        public SubscriberAction(IMediatorSubscriber subscriber, Action<IMessage> action)
        {
            Subscriber = subscriber;
            Action = action;
        }
    }

    private readonly Dictionary<Type, HashSet<SubscriberAction>> _subscriberDict = new();
    private readonly ILogger<MareMediator> _logger;
    private readonly PerformanceCollector _performanceCollector;
    private readonly object _addRemoveLock = new();

    public MareMediator(ILogger<MareMediator> logger, PerformanceCollector performanceCollector)
    {
        _logger = logger;
        _performanceCollector = performanceCollector;
    }

    public void Subscribe<T>(IMediatorSubscriber subscriber, Action<IMessage> action) where T : IMessage
    {
        lock (_addRemoveLock)
        {
            _subscriberDict.TryAdd(typeof(T), new HashSet<SubscriberAction>());

            if (!_subscriberDict[typeof(T)].Add(new(subscriber, action)))
            {
                throw new InvalidOperationException("Already subscribed");
            }

            _logger.LogDebug("Subscriber added for message {message}: {sub}", typeof(T), subscriber);
        }
    }

    public void Unsubscribe<T>(IMediatorSubscriber subscriber) where T : IMessage
    {
        lock (_addRemoveLock)
        {
            if (_subscriberDict.ContainsKey(typeof(T)))
            {
                _subscriberDict[typeof(T)].RemoveWhere(p => p.Subscriber == subscriber);
            }
        }
    }

    public void Publish(IMessage message)
    {
        if (_subscriberDict.TryGetValue(message.GetType(), out HashSet<SubscriberAction>? subscribers) && subscribers != null && subscribers.Any())
        {
            _performanceCollector.LogPerformance(this, $"Publish>{message.GetType().Name}", () =>
            {
                foreach (SubscriberAction subscriber in subscribers?.Where(s => s.Subscriber != null).ToHashSet() ?? new HashSet<SubscriberAction>())
                {
                    try
                    {
                        _performanceCollector.LogPerformance(this, $"Publish>{message.GetType().Name}+{subscriber.Subscriber.GetType().Name}", () => subscriber.Action.Invoke(message));
                    }
                    catch (Exception ex)
                    {
                        lock (_addRemoveLock)
                        {
                            var removed = _subscriberDict[message.GetType()].RemoveWhere(s => s == subscriber);
                            _logger.LogCritical(ex, "Error executing {type} for subscriber {subscriber}, removed from Mediator: {removeCount}", message.GetType().Name, subscriber.Subscriber.GetType().Name, removed);
                        }
                    }
                }
            });
        }
    }

    internal void UnsubscribeAll(IMediatorSubscriber subscriber)
    {
        lock (_addRemoveLock)
        {
            foreach (KeyValuePair<Type, HashSet<SubscriberAction>> kvp in _subscriberDict)
            {
                int unSubbed = _subscriberDict[kvp.Key]?.RemoveWhere(p => p.Subscriber == subscriber) ?? 0;
                if (unSubbed > 0)
                {
                    _logger.LogDebug("{sub} unsubscribed from {msg}", subscriber, kvp.Key.Name);
                    _logger.LogTrace("Remaining Subscribers:");
                    foreach (var item in _subscriberDict[kvp.Key])
                    {
                        _logger.LogTrace("{Key}: {item}", kvp.Key, item.Subscriber);
                    }
                }
            }
        }
    }

    public void PrintSubscriberInfo()
    {
        foreach (var kvp in _subscriberDict.ToList().SelectMany(c => c.Value.Select(v => v))
            .DistinctBy(p => p.Subscriber).OrderBy(p => p.Subscriber.GetType().FullName, StringComparer.Ordinal))
        {
            _logger.LogInformation("Subscriber {type}: {sub}", kvp.Subscriber.GetType().FullName, kvp.Subscriber.ToString());
            StringBuilder sb = new();
            sb.Append("=> ");
            foreach (var item in _subscriberDict.ToList())
            {
                if (item.Value.Any(v => v.Subscriber == kvp.Subscriber))
                {
                    sb.Append(item.Key.Name + ", ");
                }
            }
            if (!string.Equals(sb.ToString(), "=> ", StringComparison.Ordinal))
                _logger.LogInformation("{sb}", sb.ToString());
            _logger.LogInformation("---");
        }
    }

    public void Dispose()
    {
        _logger.LogTrace("Disposing {type}", GetType());
        _subscriberDict.Clear();
    }
}