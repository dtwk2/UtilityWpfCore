using System;
using System.Linq;

namespace UtilityWpf.Demo.Data.Model
{
    public class Text
    {
        public string Value { get; } = stringFaker.Value.Generate(1).Single();

        static Lazy<Bogus.Faker<string>> stringFaker = new(() =>
              {
                  return new Bogus.Faker<string>().CustomInstantiator((fake) => fake.Lorem.Sentence(10));
              });

        public override string ToString()
        {
            return Value;
        }
    }
}
