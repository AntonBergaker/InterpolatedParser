using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpolatedParsing;
public static class InterpolatedParser {

    public static void Parse(InterpolatedParseStringHandler match, string input) {
        match.Execute(input);
    }

}
