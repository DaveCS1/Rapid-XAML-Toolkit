﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class XamlElementExtractor
    {
        public static bool Parse(string xaml, List<(string element, XamlElementProcessor processor)> processors, List<IRapidXamlTag> tags)
        {
            var elementsOfInterest = processors.Select(p => p.element).ToList();

            var elementsBeingTracked = new List<TrackingElement>();

            bool isIdentifyingElement = false;
            bool isClosingElement = false;
            int currentElementStartPos = -1;

            // TODO: change these to stringbuilders to improve perf
            string lastElementName = string.Empty;
            string currentElementName = string.Empty;
            string currentElementBody = string.Empty;
            string closingElementName = string.Empty;

            // TODO: see if can only call this once per for-loop iteration
            void AddToTrackedElements(char toAppend)
            {
                for (var j = 0; j < elementsBeingTracked.Count; j++)
                {
                    elementsBeingTracked[j].ElementBody.Append(toAppend);
                }
            }

            for (int i = 0; i < xaml.Length; i++)
            {
                if (xaml[i] == '<')
                {
                    isIdentifyingElement = true;
                    currentElementStartPos = i;
                    lastElementName = currentElementName;
                    currentElementName = string.Empty;
                    currentElementBody = "<";

                    AddToTrackedElements(xaml[i]);
                }
                else if (char.IsLetterOrDigit(xaml[i]))
                {
                    if (isIdentifyingElement)
                    {
                        currentElementName += xaml[i];
                    }
                    else if (isClosingElement)
                    {
                        closingElementName += xaml[i];
                    }

                    currentElementBody += xaml[i];

                    AddToTrackedElements(xaml[i]);
                }
                else if (char.IsWhiteSpace(xaml[i]))
                {
                    currentElementBody += xaml[i];

                    AddToTrackedElements(xaml[i]);

                    if (isIdentifyingElement)
                    {
                        if (elementsOfInterest.Contains(currentElementName))
                        {
                            elementsBeingTracked.Add(new TrackingElement { StartPos = currentElementStartPos, ElementName = currentElementName, ElementBody = new StringBuilder(currentElementBody) });
                        }
                    }

                    isIdentifyingElement = false;
                }
                else if (xaml[i] == '/')
                {
                    isClosingElement = true;
                    closingElementName = string.Empty;
                    isIdentifyingElement = false;

                    currentElementBody += xaml[i];

                    AddToTrackedElements(xaml[i]);
                }
                else if (xaml[i] == '>')
                {
                    currentElementBody += xaml[i];

                    AddToTrackedElements(xaml[i]);

                    if (isIdentifyingElement)
                    {
                        if (elementsOfInterest.Contains(currentElementName))
                        {
                            elementsBeingTracked.Add(new TrackingElement { StartPos = currentElementStartPos, ElementName = currentElementName, ElementBody = new StringBuilder(currentElementBody) });
                        }

                        isIdentifyingElement = false;
                    }

                    // closing blocks can be blank or named (e.g. ' />' or '</Grid>')
                    if (isClosingElement)
                    {
                        var nameOfInterest = closingElementName;

                        if (string.IsNullOrWhiteSpace(closingElementName))
                        {
                            nameOfInterest = currentElementName;
                        }
                        else if (closingElementName == lastElementName)
                        {
                            nameOfInterest = lastElementName;
                        }

                        var toProcess = elementsBeingTracked.Where(g => g.ElementName == nameOfInterest)
                                                            .OrderByDescending(f => f.StartPos)
                                                            .Select(e => e)
                                                            .FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(toProcess.ElementName))
                        {
                            foreach (var p in processors)
                            {
                                if (p.element == toProcess.ElementName)
                                {
                                    p.processor.Process(toProcess.StartPos, toProcess.ElementBody.ToString(), tags);
                                }
                            }

                            elementsBeingTracked.Remove(toProcess);
                        }

                        // Reset this so know what we should be tracking
                        currentElementStartPos = -1;
                        isClosingElement = false;
                    }
                }
                else
                {
                    if (currentElementStartPos >= 0)
                    {
                        currentElementBody += xaml[i];

                        AddToTrackedElements(xaml[i]);
                    }
                }
            }

            return true;
        }

        private struct TrackingElement
        {
            public int StartPos { get; set; }

            public string ElementName { get; set; }

            public StringBuilder ElementBody { get; set; }
        }
    }
}
