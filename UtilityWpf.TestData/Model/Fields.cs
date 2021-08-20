using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace UtilityWpf.TestData.Model
{
    public class Fields : ReactiveObject
    {
        private string name;
        private string surname;
        private int age;
        private string phoneNumber;

        public Guid Id { get; set; }
        public string Name { get => name; set => this.RaiseAndSetIfChanged(ref name, value); }
        public string Surname { get => surname; set => this.RaiseAndSetIfChanged(ref surname, value); }
        public int Age { get => age; set => this.RaiseAndSetIfChanged(ref age, value); }
        public string PhoneNumber { get => phoneNumber; set => this.RaiseAndSetIfChanged(ref phoneNumber, value); }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id} {Name} {Surname} {Age} {PhoneNumber}";
        }
    }
}
