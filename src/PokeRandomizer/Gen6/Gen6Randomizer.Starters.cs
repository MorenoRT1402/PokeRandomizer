﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PokeRandomizer.Common.Data;
using PokeRandomizer.Common.Reference;
using PokeRandomizer.Legality;
using PokeRandomizer.Progress;
using Species = PokeRandomizer.Common.Data.Partial.Species;
using Starters = PokeRandomizer.Legality.Starters;

namespace PokeRandomizer.Gen6
{
	public abstract partial class Gen6Randomizer
	{
		public override async Task RandomizeStarters( Random taskRandom, ProgressNotifier progressNotifier, CancellationToken token )
		{
			var config = this.ValidateAndGetConfig().Starters;

			if ( !config.RandomizeStarters )
				return;

			await this.LogAsync( $"======== Beginning Starter Pokémon randomization ========{Environment.NewLine}" );
			progressNotifier?.NotifyUpdate( ProgressUpdate.StatusOnly( "Randomizing starter Pokémon..." ) );

			var starters     = await this.Game.GetStarters();
			var species      = Species.ValidSpecies.ToList();
			var speciesInfo  = await this.Game.GetPokemonInfo( true );
			var chosen       = new List<SpeciesType>();
			var speciesNames = ( await this.Game.GetTextFile( TextNames.SpeciesNames ) ).Lines;

			if ( config.StartersOnly )
			{
				species = species.Intersect( Starters.AllStarters )
								 .ToList();
			}
			else if ( !config.AllowLegendaries )
			{
				species = species.Except( Legendaries.AllLegendaries )
								 .ToList();
			}

			foreach ( var gen in starters.Generations )
			{
				for ( int i = 0; i < 3; i++ )
				{
					var thisSpecies = new List<SpeciesType>( species );

					if ( config.ElementalTypeTriangle )
					{
						// All starters are ordered by Grass, Fire, Water in all Gen 6 games
						var requiredType = i switch {
							0 => PokemonTypes.Grass,
							1 => PokemonTypes.Fire,
							_ => PokemonTypes.Water,
						};

						thisSpecies = species.Where( s => speciesInfo[ s ].HasType( requiredType ) ).ToList();
					}

					var ret = this.GetRandomSpecies( taskRandom, thisSpecies.Except( chosen ) );

					chosen.Add( ret );
					starters[ gen ][ i ] = (ushort) ret;
				}

				var generationName = gen == this.MainStarterGen ? "Main" : $"Generation {gen}";

				await this.LogAsync( $"{generationName} Starters: {string.Join( ", ", starters[ gen ].Select( s => speciesNames[ s ] ) )}" );
			}

			await this.Game.SaveStarters( starters );
			await this.LogAsync( $"{Environment.NewLine}======== Finished Starter Pokémon randomization ========{Environment.NewLine}" );
		}
	}
}