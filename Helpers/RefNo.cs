namespace InfinityCoderzz_CMSV2026.Helpers
{
    /// <summary>
    /// Builds human-friendly, display-only reference numbers from the database
    /// primary-key ids. These are purely for presentation (lists, details pages,
    /// printed/PDF invoices) — the underlying integer ids still drive all links,
    /// lookups and relationships. No database schema changes are involved.
    /// Format is a fixed prefix + zero-padded id so the same record always shows
    /// an identical reference on every page.
    /// </summary>
    public static class RefNo
    {
        public static string Bill(int id) => $"INV-{id:D6}";

        public static string Dispense(int id) => $"DSP-{id:D6}";

        public static string Rx(int id) => $"RX-{id:D6}";
    }
}
