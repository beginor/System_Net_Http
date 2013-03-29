using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpTestLib {

	public class TestEntity {

		public int Id { get; set; }

		public string Name { get; set; }

		public int Age { get; set; }

		public DateTime Birthday { get; set; }

		public static TestEntity[] CreateSamples() {
			return new[] {
				new TestEntity {
					Id = 1,
					Name = "Entity 1",
					Age = 1,
					Birthday = new DateTime(1900, 1, 1)
				},
				new TestEntity {
					Id = 2,
					Name = "Entity 2",
					Age = 1,
					Birthday = new DateTime(1900, 2, 1)
				},
				new TestEntity {
					Id = 3,
					Name = "Entity 3",
					Age = 3,
					Birthday = new DateTime(1900, 3, 1)
				}
			};
		}

	}
}
