﻿using FluentAssertions;
using NFugue.Theory;
using Xunit;

namespace NFugue.Tests.Theory
{
    public class ChordTests
    {
        [Fact]
        public void Create_chord_with_string()
        {
            var chord = new Chord("Cmaj");
            var pattern = chord.GetPattern();
            pattern.ToString().Should().BeEquivalentTo("CMAJ");
        }

        [Fact]
        public void Create_chord_with_numbered_root()
        {
            var chord = new Chord("60maj");
            var pattern = chord.GetPattern();
            pattern.ToString().Should().BeEquivalentTo("CMAJ");
        }

        [Fact]
        public void Create_chord_with_intervals()
        {
            var chord = new Chord(new Note("D5h"), new Intervals("1 3 5"));
            var pattern = chord.GetPattern();
            pattern.ToString().Should().BeEquivalentTo("D5Majh");
        }

        [Fact]
        public void Chord_first_inversion_using_number()
        {
            var chord = new Chord("C4maj") { Inversion = 1 };
            VerifyFirstInversion(chord);
        }

        [Fact]
        public void Chord_second_inversion_using_number()
        {
            var chord = new Chord("C4maj") { Inversion = 2 };
            VerifySecondInversion(chord);
        }

        [Fact]
        public void Chord_first_inversion_using_bass_note()
        {
            var chord = new Chord("C4maj");
            chord.SetBassNote("E");
            VerifyFirstInversion(chord);
        }

        [Fact]
        public void Chord_second_inversion_using_bass_note()
        {
            var chord = new Chord("C4maj");
            chord.SetBassNote("G");
            VerifySecondInversion(chord);
        }

        [Fact]
        public void Create_chord_with_notes_using_string_constructor()
        {
            var chord = Chord.FromNotes("C E G");
            chord.Should().Be(new Chord("Cmaj"));
        }

        [Fact]
        public void Create_chord_with_notes_using_string_array_constructor()
        {
            var chord = Chord.FromNotes(new[] { "Bb", "Db", "F" });
            chord.Should().Be(new Chord("Bmin^"));
        }

        [Fact]
        public void Create_chord_with_notes_using_note_array_constructor()
        {
            var chord = Chord.FromNotes(new Note[]
            {
                new Note("D"),
                new Note("F#"),
                new Note("A"),
            });
            chord.Should().Be(new Chord("Dmaj"));
        }

        [Fact]
        public void Get_chord_type_for_sus4()
        {
            var chord = new Chord("C5sus4");
            chord.GetChordType().Should().BeEquivalentTo("sus4");
        }

        [Fact]
        public void Create_three_note_chord_with_notes_in_wrong_order()
        {
            var chord = Chord.FromNotes("E G C");
            chord.Inversion.Should().Be(0);
            chord.Should().Be(new Chord("Cmaj"));
        }

        [Fact]
        public void Create_three_note_chord_with_inverted_notes()
        {
            var chord = Chord.FromNotes("E4 G4 C5");
            Assert.Equal(1, chord.Inversion);
            chord.Should().Be(new Chord("C5maj"));
        }

        [Fact]
        public void Create_four_note_chord_with_inverted_notes_first_inversion()
        {
            var chord = Chord.FromNotes("E4 G4 B4 C5");
            chord.Inversion.Should().Be(1);
            chord.Should().Be(new Chord("C5maj7^"));
        }

        [Fact]
        public void Create_four_note_chord_with_inverted_notes_second_inversion()
        {
            var chord = Chord.FromNotes("G4 C5 E5 B5");
            chord.Inversion.Should().Be(2);
            chord.Should().Be(new Chord("Cmaj7^^"));
        }

        [Fact]
        public void Create_four_note_chord_with_inverted_notes_third_inversion()
        {
            var chord = Chord.FromNotes("B4 C5 E5 G5");
            chord.Inversion.Should().Be(3);
            chord.Should().Be(new Chord("C5maj7^^^"));
            var expectedBassNote = new Note("B4") { IsOctaveExplicitlySet = false };
            chord.GetBassNote().Should().Be(expectedBassNote);
        }

        [Fact]
        public void Test_get_bass_note()
        {
            var chord = new Chord("Cmaj^");
            var expectedBassNote = new Note("E3") { IsOctaveExplicitlySet = false };
            chord.GetBassNote().Should().Be(expectedBassNote);
        }

        [Fact]
        public void Create_chord_with_notes_in_different_octaves()
        {
            var chord = Chord.FromNotes("C3 E5 G7");
            chord.Should().Be(new Chord("Cmaj"));
        }

        [Fact]
        public void Create_chord_with_many_similar_notes()
        {
            var chord = Chord.FromNotes("F3 F4 F5 A6 A5 C4 C3");
            chord.Should().Be(new Chord("Fmaj^^"));
        }

        [Fact]
        public void Test_adding_new_chord_type()
        {
            Chord.chordMap["POW"] = new Intervals("1 5");
            var chord = new Chord("Cpow");
            var notes = chord.GetNotes();

            notes[0].Should().Be(48); //C3
            notes[1].Should().Be(55); //G3
        }

        private static void VerifyFirstInversion(Chord chord)
        {
            var notes = chord.GetNotes();

            notes[0].Should().Be(60); // C4
            notes[1].Should().Be(52); // E3
            notes[2].Should().Be(55); // G3
        }

        private static void VerifySecondInversion(Chord chord)
        {
            var notes = chord.GetNotes();

            notes[0].Should().Be(60); // C4
            notes[1].Should().Be(64); // E4
            notes[2].Should().Be(55); // G3
        }
    }
}