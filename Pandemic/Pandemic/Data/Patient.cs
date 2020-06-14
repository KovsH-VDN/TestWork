using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandemic
{
    class Patient : BindableBase
    {
        public Person Person { get; set; }
        private ObservableCollection<Patient> patients;
        public ObservableCollection<Patient> Patients
        {
            get => patients;
            set
            {
                if (patients != null) return;
                patients = value;
                RaisePropertyChanged(nameof(Patients));
            }
        }
        

        public Patient(Person person, DateTime infectionTime)
        {
            Person = new Person(person);
            Person.InfectiontTimes.Add(infectionTime);
        }
    }
}
