﻿using Xunit.Extensions.Ordering.Tests.Fixtures;

namespace Xunit.Extensions.Ordering.Tests
{
	[CollectionDefinition("AF"), Collection("AF")]
	public class AssemblyFixtureTC3
	{
		private readonly AssemblyFixture1 _fixture;

		public AssemblyFixtureTC3(AssemblyFixture1 fixture)
		{
			_fixture = fixture;
		}

		[Fact]
		public void Ctor_OneInstancePerAssembly()
		{
			Assert.Equal(1, AssemblyFixture1.Count);
		}

		[Fact]
		public void Ctor_FixtureInjected()
		{
			Assert.NotNull(_fixture);
		}
	}
}