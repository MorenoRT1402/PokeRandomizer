﻿using System;
using System.IO;
using CtrDotNet.Pokemon.Structures.Common;

namespace CtrDotNet.Pokemon.Structures.Gen7
{
	public sealed class EvolutionSet : Gen6.EvolutionSet
	{
		protected override int EntrySize => 8;
		protected override int EntryCount => 8;

		public EvolutionSet( byte[] data ) : base( data ) { }

		protected override EvolutionMethod ReadMethod( BinaryReader r )
		{
			EvolutionMethod evo = base.ReadMethod( r );
			evo.Form = r.ReadSByte();
			evo.Level = r.ReadByte();
			return evo;
		}

		protected override void WriteMethod( EvolutionMethod evo, BinaryWriter w )
		{
			base.WriteMethod( evo, w );
			w.Write( (sbyte) evo.Form );
			w.Write( (byte) evo.Level );
		}
	}
}