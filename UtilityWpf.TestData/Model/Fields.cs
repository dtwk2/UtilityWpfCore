using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace UtilityWpf.TestData.Model
{
    public class Fields : ReactiveObject, IEquatable<Fields>
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
            return this.Equals(obj as Fields);
        }

        public bool Equals(Fields obj)
        {
            return $"{Id} {Name} {Surname} {Age} {PhoneNumber}".Equals(obj != null ? $"{obj.Id} {obj.Name} {obj.Surname} {obj.Age} {obj.PhoneNumber}" : "");
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(Id.ToByteArray(), 0);
        }

        public override string ToString()
        {
            return $"{Id}";
            //  return $"{Id} {Name} {Surname} {Age} {PhoneNumber}";
        }
    }
}
