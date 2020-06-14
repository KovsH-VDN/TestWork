using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandemic
{
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public IList<DateTime> InfectiontTimes { get; set; } = new List<DateTime>();
        public DateTime LastInfectionTime => SickCount != 0 ? InfectiontTimes.Last() : new DateTime();
        public int SickCount => InfectiontTimes.Count;


        public Person(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
        public Person(Person person)
        {
            Id = person.Id;
            Name = person.Name;
            Age = person.Age;
        }
    }
}
