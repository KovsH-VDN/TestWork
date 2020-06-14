using Microsoft.Win32;
using OxyPlot;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Pandemic
{
    class ViewModel : BindableBase
    {
        private readonly Model model;
        private CancellationTokenSource cancel;
        
        public ObservableCollection<Person> Persons
        {
            get => model.Persons;
            set
            {
                if (model.Persons.Equals(value)) return;
                model.Persons = value;
                RaisePropertyChanged(nameof(Persons));
            }
        }

        private ObservableCollection<Contact> contacts;
        public ObservableCollection<Contact> Contacts
        {
            get => contacts;
            set
            {
                if (contacts.Equals(value) || value == null) return;
                contacts = value;
                RaisePropertyChanged(nameof(Contacts));
            }
        }

        public ObservableCollection<Patient> Patients;
        private ObservableCollection<Person> infectedPersons;
        public ObservableCollection<Person> InfectedPersons
        {
            get => infectedPersons;
            set
            {
                if (infectedPersons.Equals(value)) return;
                infectedPersons = value;
                RaisePropertyChanged(nameof(InfectedPersons));
            }
        }



        private string info = string.Empty;
        public string Info
        {
            get => info;
            set
            {
                if (info.Equals(value)) return;
                info = value;
                RaisePropertyChanged(nameof(Info));
            }
        }

        private string avarageAge = string.Empty;
        public string AverageAge
        {
            get => avarageAge;
            set
            {
                if (avarageAge.Equals(value)) return;
                avarageAge = value;
                RaisePropertyChanged(nameof(AverageAge));
            }
        }

        private DateTime from;
        public DateTime From
        {
            get => from;
            set
            {
                if (from.Equals(value)) return;
                from = value;
                RaisePropertyChanged(nameof(From));
            }
        }

        private DateTime to;
        public DateTime To
        {
            get => to;
            set
            {
                if (to.Equals(value)) return;
                to = value;
                RaisePropertyChanged(nameof(To));
            }
        }

        private int idNullPatient;
        public int IdNullPatient
        {
            get => idNullPatient;
            set
            {
                if (idNullPatient.Equals(value)) return;
                idNullPatient = value;
                RaisePropertyChanged(nameof(IdNullPatient));
            }
        }

        private DateTime dateNullPatientInfection = DateTime.Now;
        public DateTime DateNullPatientInfection
        {
            get => dateNullPatientInfection;
            set
            {
                if (dateNullPatientInfection.Equals(value)) return;
                dateNullPatientInfection = value;
                RaisePropertyChanged(nameof(DateNullPatientInfection));
            }
        }
        private int totalInfected;
        public int TotalInfected
        {
            get => totalInfected;
            set
            {
                if (totalInfected.Equals(value)) return;
                totalInfected = value;
                RaisePropertyChanged(nameof(TotalInfected));
            }
        }
        private string peacDate = string.Empty;
        public string PeacDate
        {
            get => peacDate;
            set
            {
                if (peacDate.Equals(value)) return;
                peacDate = value;
                RaisePropertyChanged(nameof(PeacDate));
            }
        }
        private int peacInfected;
        public int PeacInfected
        {
            get => peacInfected;
            set
            {
                if (peacInfected.Equals(value)) return;
                peacInfected = value;
                RaisePropertyChanged(nameof(PeacInfected));
            }
        }
        private double lossOfEconomy;
        public double LossOfEconomy
        {
            get => lossOfEconomy;
            set
            {
                if (lossOfEconomy.Equals(value)) return;
                lossOfEconomy = value;
                RaisePropertyChanged(nameof(LossOfEconomy));
            }
        }
        private GraficsDataContext graficDataContext;
        public GraficsDataContext GraficDataContext
        {
            get => graficDataContext;
            set
            {
                if (graficDataContext.Equals(value)) return;
                graficDataContext = value;
                RaisePropertyChanged(nameof(GraficDataContext));
            }
        }
        

        public ViewModel()
        {
            model = new Model();
            model.CalculationOfEpidemicData += CommandTotalInfected.Execute;
            model.CalculationOfEpidemicData += CommandGetInfectedPersons.Execute;
            model.CalculationOfEpidemicData += CommandStatistics.Execute;

            from = DateTime.Now;
            to = DateTime.Now;

            infectedPersons = new ObservableCollection<Person>();
            contacts = new ObservableCollection<Contact>(model.Contacts);
            Patients = new ObservableCollection<Patient>();
            graficDataContext = new GraficsDataContext();
        }


        public ICommand CommandFindAverageAge
        {
            get => new DelegateCommandParametr((parametr) => AverageAge = model.AverageAge((string)parametr));
        }
        public ICommand CommandFindContactsInRange
        {
            get => new DelegateCommand(() => Contacts = new ObservableCollection<Contact>(model.ContactsInRange((From, To))));
        }
        public ICommand CommandResetListViewContacts
        {
            get => new DelegateCommand(() => Contacts = model.Contacts);
        }
        public ICommand CommandBuildTreeView
        {
            get => new DelegateCommand(() =>
            {
                cancel?.Cancel(); // Отмена работы потока по построению дерева

                ClearData(); // Очистка дерева и обнуление дат заражений людей

                Person person = model.GetPerson(IdNullPatient);
                if (person == null)
                {
                    MessageBox.Show("Нет человека с таким Id");
                    return;
                }

                Info = "Подождите, идет обработка данных.";
                cancel = new CancellationTokenSource();
                CancellationToken token = cancel.Token;

                Patient patient = new Patient(person, DateNullPatientInfection);
                person.InfectiontTimes.Add(DateNullPatientInfection);
                Patients.Add(patient);
                model.RunFillTree(Patients[0], token);
            });
        }
        public ICommand CommandTotalInfected
        {
            get => new DelegateCommand(() => TotalInfected = model.TotalInfected());
        }
        public ICommand CommandGetInfectedPersons
        {
            get => new DelegateCommand(() =>
            {
                //список заражения
                InfectedPersons = new ObservableCollection<Person>(model.InfectedPersons.OrderBy(person => person.LastInfectionTime));
                // Потери экономики с учетом, что некоторые болели несколько раз
                LossOfEconomy = InfectedPersons.Count * 8;
            });
        }
        public ICommand CommandStatistics
        {
            get => new DelegateCommand(() => 
            {
                IList<(DateTime date, int totalInfected, int currentInfected)> statistics = model.GetStatisticsPandemic();

                (DateTime date, int totalInfected, int currentInfected) peac = statistics.OrderByDescending(day => day.currentInfected).First();
                PeacDate = peac.date.ToShortDateString();
                PeacInfected = peac.currentInfected;

                GraficDataContext = new GraficsDataContext(statistics);
                Info = "Готово";
            });
        }

        
        private void ClearData()
        {
            Patients.Clear();

            ObservableCollection<Person> newPersons = new ObservableCollection<Person>();
            foreach (Person newPerson in Persons)
            {
                newPerson.InfectiontTimes = new List<DateTime>();
                newPersons.Add(newPerson);
            }

            Persons = newPersons;
        }
    }
}