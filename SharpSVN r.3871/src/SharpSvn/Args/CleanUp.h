// Copyright 2007-2008 The SharpSvn Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

#pragma once

#include "..\SvnClientArgs.h"

namespace SharpSvn {

    /// <summary>Extended parameter container for <see cref="SvnClient" />Cleanup</summary>
    /// <threadsafety static="true" instance="false"/>
    public ref class SvnCleanUpArgs : public SvnClientArgs
    {
        bool _includeExternals;
        bool _noBreakLocks;
        bool _noFixTimestamps;
        bool _noClearDavCache;
        bool _noVacuumPristines;
    public:
        SvnCleanUpArgs()
        {
        }

        property bool BreakLocks
        {
            bool get()
            {
                return !_noBreakLocks;
            }
            void set(bool value)
            {
                _noBreakLocks = !value;
            }
        }

        property bool FixTimestamps
        {
            bool get()
            {
                return !_noFixTimestamps;
            }
            void set(bool value)
            {
                _noFixTimestamps = !value;
            }
        }

        property bool ClearDavCache
        {
            bool get()
            {
                return !_noClearDavCache;
            }
            void set(bool value)
            {
                _noClearDavCache = !value;
            }
        }

        property bool VacuumPristines
        {
            bool get()
            {
                return !_noVacuumPristines;
            }
            void set(bool value)
            {
                _noVacuumPristines = !value;
            }
        }

        property bool IncludeExternals
        {
            bool get()
            {
                return _includeExternals;
            }
            void set(bool value)
            {
                _includeExternals = value;
            }
        }

        virtual property SvnCommandType CommandType
        {
            virtual SvnCommandType get() override sealed
            {
                return SvnCommandType::CleanUp;
            }
        }
    };
}
