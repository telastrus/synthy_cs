﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Core;

namespace synthy_cs
{
    public class Piano
    {
        public static readonly List<InputDevice> Devices = InputDevice.GetAll().ToList();
        public static InputDevice SelectedDevice { get; private set; } = null;
        private static int SelectedIndex = 0;
        public static bool[] PressedKeys { get; private set; } = new bool[88];
        public static Song CurrentSong = null;
        public static void Init()
        {
            if (Devices.Count == 0) return; 
            Devices[SelectedIndex].EventReceived += InputEvent;
            Devices[SelectedIndex].StartEventsListening();
            SelectedDevice = Devices[SelectedIndex];
        }

        public static void Rotate()
        {
            if (Devices.Count == 0) return;
            Devices[SelectedIndex].StopEventsListening();
            Devices[SelectedIndex].EventReceived -= InputEvent;
            SelectedIndex = (SelectedIndex + 1) % Devices.Count;
            Devices[SelectedIndex].EventReceived += InputEvent;
            Devices[SelectedIndex].StartEventsListening();
            SelectedDevice = Devices[SelectedIndex];
        }

        private static void InputEvent(object sender, MidiEventReceivedEventArgs e)
        {
            if (e.Event is NoteEvent ev)
            {
                var idx = ev.NoteNumber - 21;
                PressedKeys[idx] = ev.EventType switch
                {
                    MidiEventType.NoteOn => true,
                    MidiEventType.NoteOff => false,
                    _ => PressedKeys[idx]
                };
                CurrentSong?.SongJudgement?.RecordNoteEvent(ev);
            }
        }
    }
}
