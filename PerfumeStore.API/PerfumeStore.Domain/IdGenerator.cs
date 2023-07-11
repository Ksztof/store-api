using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain
{
	public static class IdGenerator
	{
		private static int _currentId;

		public static int GetNextId() => ++_currentId;
	}
}
