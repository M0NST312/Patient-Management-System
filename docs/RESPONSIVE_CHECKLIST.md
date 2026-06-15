Responsive Checklist for ClinicSystem

Quick manual verification steps (use browser DevTools device toolbar or a real phone):

Pages to test:
- Dashboard (Index)
- Patients: Index, Create, Edit, Details
- Visits: Index, Create, Details
- Billing: Index, Create, Details
- Admin: Users Index, Create, Edit

Checklist:
- Navigation
  - Header has a visible menu button on small screens.
  - Sidebar opens as an overlay and closes when tapping outside.
- Forms
  - Inputs and selects span full width on small screens.
  - Labels are readable and stacked above inputs.
  - Primary buttons are reachable and tappable.
- Tables / Lists
  - Wide tables scroll horizontally inside their card.
  - Important columns (action buttons) remain usable.
- Touch targets & spacing
  - Buttons and interactive rows are at least 40px tall.
  - Spacing between controls prevents mis-taps.
- Visual
  - No horizontal overflow; pages don't require horizontal scrolling except inside table wrappers.
  - Fonts remain legible at small breakpoints.

Test breakpoints (examples):
- Phone narrow: 360x800
- Phone large: 412x915
- Small tablet: 768x1024

Notes / How to fix common issues:
- If a table's important columns get hidden, consider removing less important columns on small screens or move actions into a dropdown per-row.
- For persistent sidebars, ensure they become off-canvas at <=768px and provide a header toggle.
- If forms still look cramped, decrease horizontal gutters or convert multi-column rows into stacked single-column at <=576px.

Done-by: Developer
Date: 2026-06-15
