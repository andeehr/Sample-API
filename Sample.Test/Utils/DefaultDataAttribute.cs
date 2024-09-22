using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Sample.Test.Utils
{
    public class DefaultDataAttribute : AutoDataAttribute
    {
        public DefaultDataAttribute()
            : base(FixtureFactory([]))
        {
        }

        public DefaultDataAttribute(params Type[] customizationTypes)
            : base(FixtureFactory(customizationTypes ?? []))
        {
        }

        private static Func<IFixture> FixtureFactory(Type[] customizationTypes)
        {
            return delegate
            {
                var fixture = new Fixture();

                fixture.Customize(new CompositeCustomization(
                    new ICustomization[1]
                    {
                        new DefaultCustomization()
                    }
                    .Concat(customizationTypes.Select(t => (ICustomization)Activator.CreateInstance(t)!))
                ));

                fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                    .ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));

                fixture.Behaviors.Add(new OmitOnRecursionBehavior());

                return fixture;
            };
        }

        public class DefaultCustomization : CompositeCustomization
        {
            public DefaultCustomization()
                : base(new AutoMoqCustomization())
            {
            }
        }
    }
}