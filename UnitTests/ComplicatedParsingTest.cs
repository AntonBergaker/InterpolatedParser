using InterpolatedParsing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests;
internal class ComplicatedParsingTest {

    private record Dice(int Count, string Color) : IParsable<Dice> {
        public static Dice Parse(string input, IFormatProvider? provider) {
            int count = 0;
            string color = null!;

            InterpolatedParser.Parse(
                $"{count} {color}",
                input);

            return new(count, color);
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Dice result) {
            throw new NotImplementedException();
        }
    }

    private record Round(Dice[] Dice) : IParsable<Round> {
        public static Round Parse(string input, IFormatProvider? provider) {
            Dice[] dice = null!;

            InterpolatedParser.Parse(
                $"{dice:', '}",
                input);

            return new(dice);
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Round result) {
            throw new NotImplementedException();
        }
    }

    private record Game(int index, Round[] Rounds) : IParsable<Game> {
        public static Game Parse(string input, IFormatProvider? provider) {
            int index = 0;
            Round[] rounds = null!;

            InterpolatedParser.Parse(
                $"Game {index}: {rounds:'; '}",
                input);

            return new Game(index, rounds);
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Game result) {
            throw new NotImplementedException();
        }
    }

    [Test]
    public void ComplicatedNestedInput() {

        // Taken from Advent of Code 2023 Day 2
        var input = """
            Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
            Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
            Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
            Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
            """;

        List<Game> games = default!;

        InterpolatedParser.Parse(
            $"{games:\"\n\"}",
            input);

        Assert.AreEqual(5, games.Count);
        Assert.AreEqual(3, games[0].Rounds.Length);
        Assert.AreEqual("blue", games[0].Rounds[0].Dice[0].Color);
    }


}
