﻿//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Diagnostics;

namespace Moq
{
	/// <summary>
	/// A <see cref="DefaultValueProvider"/> that returns an empty default value 
	/// for non-mockeable types, and mocks for all other types (interfaces and 
	/// non-sealed classes) that can be mocked.
	/// </summary>
	internal sealed class MockDefaultValueProvider : LookupOrFallbackDefaultValueProvider
	{
		internal MockDefaultValueProvider()
		{
		}

		internal override DefaultValue Kind => DefaultValue.Mock;

		protected override object GetFallbackDefaultValue(Type type, Mock mock)
		{
			Debug.Assert(type != null);
			Debug.Assert(type != typeof(void));
			Debug.Assert(mock != null);

			var emptyValue = DefaultValueProvider.Empty.GetDefaultValue(type, mock);
			if (emptyValue != null)
			{
				return emptyValue;
			}
			else if (type.IsMockeable())
			{
				// Create a new mock to be placed to InnerMocks dictionary if it's missing there
				var mockType = typeof(Mock<>).MakeGenericType(type);
				Mock newMock = (Mock)Activator.CreateInstance(mockType, mock.Behavior);
				newMock.DefaultValueProvider = mock.DefaultValueProvider;
				newMock.CallBase = mock.CallBase;
				newMock.Switches = mock.Switches;
				return newMock.Object;
			}
			else
			{
				return null;
			}
		}
	}
}
