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
    class Model
    {
        public event Action<object> CalculationOfEpidemicData;


        private string[] fileNames = new string[]
        {
            "big_data.xml",
            "small_data.xml"
        };
        public ObservableCollection<Person> Persons { get; set; }
        public ObservableCollection<Contact> Contacts { get; set; }
        public ObservableCollection<Person> InfectedPersons { get; set; }
        public Virus Virus { get; set; }


        public Model()
        {
            Persons = new ObservableCollection<Person>();
            Contacts = new ObservableCollection<Contact>();
            InfectedPersons = new ObservableCollection<Person>();
            Virus = new Virus();


            foreach (string fileName in fileNames)
                LoadData(fileName);
        }


        public void LoadData(string fileName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            XmlElement root = document.DocumentElement;

            foreach (XmlNode node in root)
                foreach (XmlNode childNode in node)
                {
                    if (childNode.Name == "Member")
                        Persons.Add(new Person(
                            int.Parse(childNode.ChildNodes[0].FirstChild.Value),
                            childNode.ChildNodes[1].FirstChild.Value,
                            int.Parse(childNode.ChildNodes[2].FirstChild.Value)
                            ));

                    if (childNode.Name == "MapOfContact")
                        Contacts.Add(new Contact(
                            DateTime.Parse(childNode.ChildNodes[0].FirstChild.Value),
                            DateTime.Parse(childNode.ChildNodes[1].FirstChild.Value),
                            int.Parse(childNode.ChildNodes[2].FirstChild.Value),
                            int.Parse(childNode.ChildNodes[3].FirstChild.Value)
                            ));
                }
        }
        public string AverageAge(string name)
        {
            IEnumerable<Person> foundPersons = Persons.Where(person => person.Name == name);
            if (foundPersons.Count() != 0)
                return $"Средний возраст: {foundPersons.Average(person => person.Age).ToString()}";
            return "Людей с данным именем не найдено.";
        }
        public IEnumerable<Contact> ContactsInRange((DateTime from, DateTime to) range)
        {
            return 
                Contacts.Where(contact => 
                contact.From > range.from &&
                contact.To < range.to &&
                (contact.To - contact.From).Minutes > 10);
        }
        public void RunFillTree(Patient currentPatient, CancellationToken token)
        {
            InfectedPersons.Clear();

            Task task = Task.Run(() => Awaiting(currentPatient, token), token);
        }
        private async void Awaiting(Patient currentPatient, CancellationToken token)
        {
            await FillTreeBranch(currentPatient, token);

            CalculationOfEpidemicData.Invoke(new object());
        }
        public async Task FillTreeBranch(Patient currentPatient, CancellationToken token)
        {

            if (currentPatient.Patients == null)
                currentPatient.Patients = new ObservableCollection<Patient>(GetCurrentsInfected(currentPatient));

            if (currentPatient.Patients.Count != 0)
                for (int i = 0; i < currentPatient.Patients.Count; ++i)
                {
                    token.ThrowIfCancellationRequested();
                    await FillTreeBranch(currentPatient.Patients[i], token);
                }
        }
        public IEnumerable<Patient> GetCurrentsInfected(Patient patient)
        {
            InfectedPersons.Add(patient.Person);

            IList<Contact> infected;
            infected = Contacts.
                Where(contact =>
                (contact.Member1_ID == patient.Person.Id || contact.Member2_ID == patient.Person.Id) &&
                (contact.To - contact.From) >= Virus.ContactDuration &&
                contact.From > patient.Person.LastInfectionTime + Virus.LatentPeriod &&
                contact.From < patient.Person.LastInfectionTime + Virus.LatentPeriod + Virus.DiseasePeriod).
                ToList();

            IList<Patient> patients = new List<Patient>();

            foreach (Contact contact in infected)
            {
                int id = patient.Person.Id == contact.Member1_ID ? contact.Member2_ID : contact.Member1_ID;
                Person infectedPerson;

                infectedPerson = Persons.
                    Where(person =>
                    person.Id == id &&
                    contact.From > (person.LastInfectionTime + Virus.LatentPeriod + Virus.DiseasePeriod + Virus.ImmunityPeriod)).
                    FirstOrDefault();

                if (infectedPerson == null) continue;

                DateTime infectionTime = contact.From + Virus.ContactDuration;
                patients.Add(new Patient(infectedPerson, infectionTime));
                infectedPerson.InfectiontTimes.Add(infectionTime);
            }

            return patients;
        }
        public IList<(DateTime date, int totalInfected, int currentInfected)> GetStatisticsPandemic()
        {
            DateTime beginPandemic = InfectedPersons.Min(person => person.LastInfectionTime);
            DateTime endPandemic = InfectedPersons.Max(person => person.LastInfectionTime);
            TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);

            IList<(DateTime date, int totalInfected, int currentInfected)> statistics = new List<(DateTime, int, int)>();

            for (DateTime currentDate = beginPandemic; currentDate <= endPandemic; currentDate += oneDay)
                statistics.Add(
                    (currentDate,
                    InfectedPersons.Where(person => person.LastInfectionTime >= currentDate && person.LastInfectionTime < (currentDate + oneDay)).Count(),
                    InfectedPersons.Where(person => person.LastInfectionTime <= currentDate && person.LastInfectionTime + Virus.LatentPeriod + Virus.DiseasePeriod > currentDate).Count()
                    ));

            return statistics;
        }

        
        public Person GetPerson(int Id) => Persons.Where(person => person.Id == Id).FirstOrDefault();
        public int TotalInfected()=>Persons.Where(person => person.SickCount > 0).Count();
    }
}