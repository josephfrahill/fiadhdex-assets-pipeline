using System;
using System.Collections.Generic;
using System.Text;

namespace Models;

public record FilterResult(bool Passed, string? Reason);