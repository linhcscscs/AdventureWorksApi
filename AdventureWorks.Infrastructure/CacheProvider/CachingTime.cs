using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Infrastructure.CacheProvider
{
    class CachingTime
    {
        public const double CACHING_TIME_DEFAULT_IN_24_HOURS = 1440D;
        public const double CACHING_TIME_DEFAULT_IN_5_MINUTES = 5D;
        public const double CACHING_TIME_DEFAULT_IN_0_5_MINUTES = 0.5D;
        public const double CACHING_TIME_DEFAULT_IN_0_2_5_MINUTES = 0.25D;//15 second
        public const double CACHING_TIME_DEFAULT_IN_1_MINUTES = 1D; // 1 phut
        public const double CACHING_TIME_DEFAULT_IN_10_MINUTES = 10D; // 10 phut
        public const double CACHING_TIME_DEFAULT_IN_20_MINUTES = 20D;
        public const double CACHING_TIME_DEFAULT_IN_30_MINUTES = 30D;
    }
}
