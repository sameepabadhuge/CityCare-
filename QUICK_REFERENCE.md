# CityCare - Quick Reference Guide

## ?? UI COMPONENTS QUICK REFERENCE

### BUTTONS

```html
<!-- Primary Button (Yellow) -->
<a class="btn btn-cc-primary">
    <i class="bi bi-plus-lg"></i> Add Item
</a>

<!-- Outline Button (Bordered) -->
<a class="btn btn-cc-outline">
    <i class="bi bi-arrow-left"></i> Back
</a>

<!-- Logout Button -->
<button type="submit" class="btn btn-cc-logout btn-sm">
    <i class="bi bi-box-arrow-right"></i> Logout
</button>
```

### BADGES

```html
<!-- Pending Status -->
<span class="badge-cc badge-pending">
    <i class="bi bi-hourglass-split"></i> Pending
</span>

<!-- In Progress Status -->
<span class="badge-cc badge-inprogress">
    <i class="bi bi-arrow-repeat"></i> In Progress
</span>

<!-- Resolved Status -->
<span class="badge-cc badge-resolved">
    <i class="bi bi-check-circle-fill"></i> Resolved
</span>

<!-- Category Badge -->
<span class="badge-category">Infrastructure</span>
```

### CARDS

```html
<!-- Basic Card -->
<div class="card-cc">
    <div class="card-body p-4">
        <!-- Content -->
    </div>
</div>

<!-- Status Card -->
<div class="card-cc status-card">
    <div class="card-body p-4">
        <!-- Content -->
    </div>
</div>

<!-- No Hover Effect -->
<div class="card-cc no-hover">
    <div class="card-body p-4">
        <!-- Content -->
    </div>
</div>
```

### TABLES

```html
<!-- Table Container -->
<div class="table-cc-container">
    <div class="table-cc-header">
        <h5 class="mb-0">Title</h5>
    </div>
    
    <div class="table-responsive">
        <table class="table table-hover table-cc align-middle mb-0">
            <thead>
                <tr>
                    <th>Column 1</th>
                    <th>Column 2</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class="fw-semibold">Data</td>
                    <td>Data</td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
```

### FORMS

```html
<!-- Form Container -->
<div class="form-cc-container">
    <form>
        <!-- Form Fields -->
    </form>
</div>

<!-- Form Label -->
<label class="form-label-cc">Field Label</label>

<!-- Text Input -->
<input class="form-control-cc w-100" placeholder="Placeholder..." />

<!-- Select -->
<select class="form-select-cc w-100">
    <option>Option 1</option>
</select>

<!-- Helper Text -->
<div class="form-text-cc">Helper text here</div>

<!-- Validation Error -->
<span class="text-danger small">Error message</span>
```

### ALERTS

```html
<!-- Success Alert -->
<div class="alert-cc success">
    <i class="bi bi-check-circle-fill"></i>
    <span>Success message</span>
    <button type="button" class="btn-close ms-auto" data-bs-dismiss="alert"></button>
</div>

<!-- Error Alert -->
<div class="alert-cc error">
    <i class="bi bi-exclamation-triangle-fill"></i>
    <span>Error message</span>
</div>
```

### PAGE HEADERS

```html
<!-- Admin Header -->
<div class="admin-header mb-4">
    <h1>Page Title</h1>
    <p>Page subtitle or description</p>
</div>

<!-- Citizen Page Header -->
<div class="table-cc-header">
    <div class="d-flex justify-content-between align-items-center">
        <div>
            <h1 class="page-title">Page Title</h1>
            <p class="page-subtitle">Subtitle here</p>
        </div>
        <a class="btn btn-cc-primary">Action</a>
    </div>
</div>
```

### NOTIFICATIONS

```html
<!-- Notification Item -->
<div class="notify-item unread">
    <a href="#" style="flex: 1; display: flex; align-items: center; gap: 1rem;">
        <div class="notify-icon">
            <i class="bi bi-bell"></i>
        </div>
        <div class="notify-content">
            <h5 class="notify-title">
                <span class="notify-badge"></span>
                Title
            </h5>
            <p class="notify-msg">Message here</p>
        </div>
        <div class="notify-time">2h ago</div>
    </a>
</div>
```

### INFO BOXES

```html
<!-- Info Box -->
<div class="info-box-cc">
    <div>
        <i class="bi bi-geo-alt" style="font-size: 1.5rem; color: var(--yellow);"></i>
    </div>
    <div>
        <span class="info-label">Label</span>
        <div class="info-value">Value here</div>
        <div class="small text-muted">Details</div>
    </div>
</div>
```

---

## ?? COLOR VARIABLES

All colors are defined as CSS variables in `:root`:

```css
--navy: #0B2A4A           /* Primary dark blue */
--navy-2: #123B63         /* Secondary dark blue */
--yellow: #FFC400         /* Primary accent */
--yellow-2: #FFB300       /* Darker yellow */
--bg: #F6F8FB             /* Light background */
--text: #0F172A           /* Primary text */
--muted: #64748B          /* Secondary text */
--card: #FFFFFF           /* Card background */
--border: #E6EAF2         /* Border color */
--success: #10B981        /* Success color */
--pending: #EF4444        /* Pending color */
--inprogress: #3B82F6     /* In progress color */
```

Use them like:
```css
background: var(--yellow);
color: var(--text);
border-color: var(--border);
```

---

## ?? RESPONSIVE BREAKPOINTS

Bootstrap breakpoints used:
- `xs`: < 576px (mobile)
- `sm`: ? 576px (landscape mobile)
- `md`: ? 768px (tablet)
- `lg`: ? 992px (desktop)
- `xl`: ? 1200px (large desktop)

Example:
```html
<div class="col-md-6 col-lg-4">Content</div>
```

---

## ?? COMMON PATTERNS

### SECTION WITH BORDER DIVIDER
```html
<div class="d-flex justify-content-between align-items-start mb-4 pb-3 border-bottom border-2" style="border-color: var(--border);">
    <div>
        <h1 class="page-title">Title</h1>
        <p class="page-subtitle">Subtitle</p>
    </div>
    <a class="btn btn-cc-primary">Action</a>
</div>
```

### EMPTY STATE
```html
<div style="text-align: center; padding: 3rem 1.5rem;">
    <div style="font-size: 3rem; margin-bottom: 1rem;">??</div>
    <h5 style="font-weight: 700; color: var(--muted);">No items found</h5>
    <p class="text-muted mb-0">Create your first item.</p>
</div>
```

### STICKY SIDEBAR
```html
<div class="sticky-top" style="top: 2rem;">
    <!-- Sidebar content -->
</div>
```

### FLEX LAYOUT WITH BUTTONS
```html
<div class="d-flex gap-2 justify-content-end">
    <a class="btn btn-cc-outline">Cancel</a>
    <button class="btn btn-cc-primary">Save</button>
</div>
```

---

## ?? ICON GUIDELINES

Using Bootstrap Icons (CSS classes):

```html
<!-- Navigation -->
<i class="bi bi-house"></i>        <!-- Home -->
<i class="bi bi-bell"></i>         <!-- Notifications -->
<i class="bi bi-box-arrow-right"></i> <!-- Logout -->
<i class="bi bi-arrow-left"></i>   <!-- Back -->

<!-- Actions -->
<i class="bi bi-plus-lg"></i>      <!-- Add -->
<i class="bi bi-pencil"></i>       <!-- Edit -->
<i class="bi bi-trash"></i>        <!-- Delete -->
<i class="bi bi-eye"></i>          <!-- View -->

<!-- Status -->
<i class="bi bi-check-circle"></i>      <!-- Success -->
<i class="bi bi-hourglass-split"></i>   <!-- Pending -->
<i class="bi bi-arrow-repeat"></i>      <!-- In progress -->
<i class="bi bi-exclamation-triangle"></i> <!-- Warning -->

<!-- Others -->
<i class="bi bi-image"></i>        <!-- Images -->
<i class="bi bi-geo-alt"></i>      <!-- Location -->
<i class="bi bi-telephone"></i>    <!-- Phone -->
<i class="bi bi-envelope"></i>     <!-- Email -->
```

---

## ?? DEPRECATED CLASSES (Don't Use)

- ? `.btn-logout` ? Use `.btn-cc-logout` instead
- ? `.btn-primary-cc` ? Use `.btn-cc-primary` instead
- ? `.btn-outline-cc` ? Use `.btn-cc-outline` instead
- ? Bootstrap `.btn-primary` ? Use `.btn-cc-primary` instead
- ? Bootstrap `.badge bg-*` ? Use `.badge-cc badge-*` instead

---

## ? BEST PRACTICES

1. **Always use semantic HTML**
   ```html
   <button type="submit"> instead of <button>
   <a href=""> instead of <a onclick="">
   ```

2. **Include ARIA labels**
   ```html
   <button aria-label="Close menu">×</button>
   ```

3. **Use Bootstrap grid for responsiveness**
   ```html
   <div class="row g-3">
       <div class="col-md-6">Half width on tablet+</div>
       <div class="col-md-6">Half width on tablet+</div>
   </div>
   ```

4. **Group related elements with gap utility**
   ```html
   <div class="d-flex gap-2">
       <button class="btn">Button 1</button>
       <button class="btn">Button 2</button>
   </div>
   ```

5. **Use form-check for checkboxes/radios**
   ```html
   <div class="form-check">
       <input class="form-check-input" type="checkbox" id="check">
       <label class="form-check-label" for="check">Label</label>
   </div>
   ```

---

## ?? FILE STRUCTURE

```
CityCare/
??? Views/
?   ??? Shared/
?   ?   ??? _Layout.cshtml          ? Main layout
?   ??? Admin/
?   ?   ??? Dashboard.cshtml
?   ?   ??? Cities.cshtml
?   ?   ??? Departments.cshtml
?   ?   ??? StaffCodes.cshtml
?   ??? Staff/
?   ?   ??? Dashboard.cshtml
?   ?   ??? Details.cshtml
?   ??? Issue/
?   ?   ??? Dashboard.cshtml
?   ?   ??? Create.cshtml
?   ?   ??? Details.cshtml
?   ??? Account/
?   ?   ??? Login.cshtml
?   ?   ??? CitizenRegister.cshtml
?   ?   ??? StaffRegister.cshtml
?   ??? Notification/
?       ??? Index.cshtml
??? wwwroot/css/
    ??? site.css                   ? Main styles
    ??? citizen-theme.css          ? Citizen/Staff theme
    ??? landing.css                ? Public pages
```

---

## ?? STYLING WORKFLOW

### To create a new page:
1. Choose a wrapper: `.admin-page` or `.citizen-page`
2. Add header: `.admin-header` or `.table-cc-header`
3. Add content with `.card-cc` or `.table-cc-container`
4. Use consistent buttons: `.btn-cc-primary`, `.btn-cc-outline`
5. Test on mobile

### To add a new component:
1. Define it in the appropriate CSS file
2. Use `.component-name` naming
3. Document it with classes
4. Add examples to this guide

---

## ?? QUICK START FOR DEVELOPERS

When adding a new page, copy this template:

```html
@{
    ViewData["Title"] = "Page Title";
}

<div class="admin-page">  <!-- or citizen-page -->
    <div class="container">
        
        <!-- Header -->
        <div class="admin-header mb-4">
            <h1>Page Title</h1>
            <p>Page description</p>
        </div>

        <!-- Content -->
        <div class="card-cc">
            <div class="card-body p-4">
                <!-- Your content here -->
            </div>
        </div>

    </div>
</div>
```

That's it! Your page will automatically be styled consistently. ??

