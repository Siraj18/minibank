using System;

namespace Minibank.Core
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}