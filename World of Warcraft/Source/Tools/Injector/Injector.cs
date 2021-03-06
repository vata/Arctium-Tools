﻿/*
 * Copyright (C) 2012-2013 Arctium <http://arctium.org>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Arctium_Injector
{
    class Injector
    {
        static void Main(string[] args)
        {
            // Default World of Warcraft process name
            var processName = Environment.Is64BitProcess ? "WoW-64" : "WoW";
            var dll = "AwpsHost.dll";

            if (args.Length == 1)
                processName = Path.GetFileNameWithoutExtension(args[0]);

            if (args.Length == 2)
                dll = args[1];

            var waiting = true;

            while (waiting)
            {
                var processList = Process.GetProcessesByName(processName);

                if (processList.Length > 1)
                {
                    // Several process... Show menu and wait for valid entry
                    var process         = processList.Length;
                    var selectedProcess = 0;
                    var looping         = true;
                    var validProcess    = false;

                    Console.WriteLine("Found {0} processes. Which one to sniff?", process);

                    for (var i = 0; i < process; i++)
                    {
                        if (Functions.IsProcessAlreadyInjected(processList[i], dll))
                            Console.WriteLine("[!][injected] ({0})", processList[i].MainWindowTitle);
                        else
                            Console.WriteLine("[?][{0}] - {1} ({2})", i + 1, processList[i].Id, processList[i].MainWindowTitle);
                    }

                    Console.WriteLine("[?][0] - Exit");

                    while (looping)
                    {
                        try
                        {
                            selectedProcess = Convert.ToInt32(Console.ReadLine());

                            if (selectedProcess == 0)
                            {
                                looping = false;
                            }
                            else if (selectedProcess <= process)
                            {
                                if (Functions.IsProcessAlreadyInjected(processList[selectedProcess - 1], dll))
                                {
                                    Console.WriteLine("Process already injected! Choose another one...");
                                }
                                else
                                {
                                    looping = false;
                                    validProcess = true;
                                }
                            }
                        }
                        catch(Exception exception)
                        {
                            looping = false;

                            Console.WriteLine(exception.Message);
                        }
                    }

                    if (validProcess)
                        Functions.Inject(processList[selectedProcess - 1], dll);
                }
                else if (processList.Length == 1)
                {
                    // Only one process was found
                    if (Functions.IsProcessAlreadyInjected(processList[0], dll))
                        Console.WriteLine("Process already injected!");
                    else
                        Functions.Inject(processList[0], dll);
                }
                else
                {
                    Console.WriteLine("Waiting for {0} process...", processName);
                    Thread.Sleep(1500);
                }

                waiting = (processList.Length <= 0);
            }
        }
    }
}
