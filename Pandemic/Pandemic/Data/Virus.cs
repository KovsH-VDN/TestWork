using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandemic
{
    class Virus
    {
        public TimeSpan LatentPeriod => new TimeSpan(4, 0, 0, 0); // Инкубационный период
        public TimeSpan ContactDuration => new TimeSpan(0, 5, 0);  // Минимальное время контакта для заражения
        public TimeSpan DiseasePeriod => new TimeSpan(7, 0, 0, 0);  // Период болезни
        public TimeSpan ImmunityPeriod => new TimeSpan(14, 0, 0, 0);  // Период устойчивости к болезни

    }
}
