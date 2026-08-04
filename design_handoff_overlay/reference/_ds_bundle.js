/* @ds-bundle: {"format":4,"namespace":"CortinhosDesignSystem_c0acc5","components":[{"name":"Badge","sourcePath":"components/core/Badge.jsx"},{"name":"Icon","sourcePath":"components/core/Icon.jsx"},{"name":"ShortcutTile","sourcePath":"components/core/ShortcutTile.jsx"},{"name":"ContextMenu","sourcePath":"components/feedback/ContextMenu.jsx"},{"name":"Dialog","sourcePath":"components/feedback/Dialog.jsx"},{"name":"Tooltip","sourcePath":"components/feedback/Tooltip.jsx"},{"name":"Button","sourcePath":"components/forms/Button.jsx"},{"name":"Checkbox","sourcePath":"components/forms/Checkbox.jsx"},{"name":"Select","sourcePath":"components/forms/Select.jsx"},{"name":"TextField","sourcePath":"components/forms/TextField.jsx"},{"name":"ThemeToggle","sourcePath":"components/forms/ThemeToggle.jsx"},{"name":"Notch","sourcePath":"components/surface/Notch.jsx"}],"sourceHashes":{"components/core/Badge.jsx":"92f4d432ee01","components/core/Icon.jsx":"3187be408e0b","components/core/ShortcutTile.jsx":"fc24283b4f92","components/feedback/ContextMenu.jsx":"352ffdee2f1a","components/feedback/Dialog.jsx":"a6ed467aec8a","components/feedback/Tooltip.jsx":"5e45880ffb56","components/forms/Button.jsx":"f6046d008488","components/forms/Checkbox.jsx":"7cf88d8bdb84","components/forms/Select.jsx":"a51326b0428f","components/forms/TextField.jsx":"99624fdf9830","components/forms/ThemeToggle.jsx":"96c1113394fa","components/surface/Notch.jsx":"fec1a3a8f3c5","components/surface/notch-spec.jsx":"4c72df4b7b63","ui_kits/launcher/LauncherApp.jsx":"42f393df6e23","ui_kits/overlay/Feasibility.jsx":"1b5eb865e17e","ui_kits/overlay/Islands.jsx":"3a409763d5e6","ui_kits/overlay/OverlayApp.jsx":"ed6d7cc33326"},"inlinedExternals":[],"unexposedExports":[]} */

(() => {

const __ds_ns = (window.CortinhosDesignSystem_c0acc5 = window.CortinhosDesignSystem_c0acc5 || {});

const __ds_scope = {};

(__ds_ns.__errors = __ds_ns.__errors || []);

// components/core/Badge.jsx
try { (() => {
const TONES = {
  neutral: {
    bg: 'var(--surface-3)',
    fg: 'var(--text-secondary)'
  },
  accent: {
    bg: 'var(--accent-soft)',
    fg: 'var(--accent)'
  },
  success: {
    bg: 'var(--success-soft)',
    fg: 'var(--success)'
  },
  danger: {
    bg: 'var(--danger-soft)',
    fg: 'var(--danger)'
  }
};
function Badge({
  children,
  tone = 'neutral',
  mono = false,
  icon,
  style
}) {
  const t = TONES[tone] || TONES.neutral;
  return /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 4,
      padding: '2px 8px',
      borderRadius: 'var(--radius-pill)',
      background: t.bg,
      color: t.fg,
      fontFamily: mono ? 'var(--font-mono)' : 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-semibold)',
      letterSpacing: mono ? 0 : '.02em',
      lineHeight: 1.4,
      whiteSpace: 'nowrap',
      ...style
    }
  }, icon, children);
}
Object.assign(__ds_scope, { Badge });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/core/Badge.jsx", error: String((e && e.message) || e) }); }

// components/core/Icon.jsx
try { (() => {
function _extends() { return _extends = Object.assign ? Object.assign.bind() : function (n) { for (var e = 1; e < arguments.length; e++) { var t = arguments[e]; for (var r in t) ({}).hasOwnProperty.call(t, r) && (n[r] = t[r]); } return n; }, _extends.apply(null, arguments); }
/* Cortinhos icon set — Solar **Bold Duotone** (CC-BY), with the mic pair from
   Hugeicons (Solar has no muted-mic glyph). Bodies are inlined so the bundle
   has no runtime CDN dependency. All 24×24, `currentColor`.

   Bold Duotone = a solid silhouette at opacity .5 plus a solid detail at full
   opacity, so one color reads as two tones. The mic pair is stroke-based
   (Hugeicons) — a deliberate exception, documented in readme ICONOGRAPHY. */
const PATHS = {
  // shortcut types
  app: '<path fill="currentColor" d="M2 6.21c0-1.984 0-2.977.659-3.593S4.379 2 6.5 2s3.182 0 3.841.617C11 3.233 11 4.226 11 6.21v11.58c0 1.984 0 2.977-.659 3.593S8.621 22 6.5 22s-3.182 0-3.841-.617C2 20.767 2 19.774 2 17.79z" opacity=".5"/><path fill="currentColor" d="M13 15.4c0-2.074 0-3.111.659-3.756S15.379 11 17.5 11s3.182 0 3.841.644C22 12.29 22 13.326 22 15.4v2.2c0 2.074 0 3.111-.659 3.756S19.621 22 17.5 22s-3.182 0-3.841-.644C13 20.71 13 19.674 13 17.6zm0-9.9c0-1.087 0-1.63.171-2.06a2.3 2.3 0 0 1 1.218-1.262C14.802 2 15.327 2 16.375 2h2.25c1.048 0 1.573 0 1.986.178c.551.236.99.69 1.218 1.262c.171.43.171.973.171 2.06s0 1.63-.171 2.06a2.3 2.3 0 0 1-1.218 1.262C20.198 9 19.673 9 18.625 9h-2.25c-1.048 0-1.573 0-1.986-.178a2.3 2.3 0 0 1-1.218-1.262C13 7.13 13 6.587 13 5.5"/>',
  folder: '<path fill="currentColor" d="M22 14v-2.202c0-2.632 0-3.949-.77-4.804a3 3 0 0 0-.224-.225C20.151 6 18.834 6 16.202 6h-.374c-1.153 0-1.73 0-2.268-.153a4 4 0 0 1-.848-.352C12.224 5.224 11.816 4.815 11 4l-.55-.55c-.274-.274-.41-.41-.554-.53a4 4 0 0 0-2.18-.903C7.53 2 7.336 2 6.95 2c-.883 0-1.324 0-1.692.07A4 4 0 0 0 2.07 5.257C2 5.626 2 6.068 2 6.95V14c0 3.771 0 5.657 1.172 6.828S6.229 22 10 22h4c3.771 0 5.657 0 6.828-1.172S22 17.771 22 14" opacity=".5"/><path fill="currentColor" d="M12.25 10a.75.75 0 0 1 .75-.75h5a.75.75 0 0 1 0 1.5h-5a.75.75 0 0 1-.75-.75"/>',
  url: '<path fill="currentColor" fill-rule="evenodd" d="M8 2.25A6.75 6.75 0 0 0 2.969 13.5a.75.75 0 0 0 1.118-1A5.25 5.25 0 0 1 8 3.75h4a5.25 5.25 0 1 1 0 10.5h-2a.75.75 0 0 0 0 1.5h2a6.75 6.75 0 0 0 0-13.5z" clip-rule="evenodd"/><path fill="currentColor" d="M6.75 15c0-2.9 2.35-5.25 5.25-5.25h2a.75.75 0 0 0 0-1.5h-2a6.75 6.75 0 0 0 0 13.5h4a6.75 6.75 0 0 0 5.031-11.25a.75.75 0 0 0-1.118 1A5.25 5.25 0 0 1 16 20.25h-4A5.25 5.25 0 0 1 6.75 15" opacity=".5"/>',
  command: '<path fill="currentColor" d="M2 12c0-4.714 0-7.071 1.464-8.536C4.93 2 7.286 2 12 2s7.071 0 8.535 1.464C22 4.93 22 7.286 22 12s0 7.071-1.465 8.535C19.072 22 16.714 22 12 22s-7.071 0-8.536-1.465C2 19.072 2 16.714 2 12" opacity=".5"/><path fill="currentColor" d="M13.488 6.446a.75.75 0 0 1 .53.918l-2.588 9.66a.75.75 0 0 1-1.449-.389l2.589-9.659a.75.75 0 0 1 .918-.53M14.97 8.47a.75.75 0 0 1 1.06 0l.209.208c.635.635 1.165 1.165 1.529 1.642c.384.504.654 1.036.654 1.68s-.27 1.176-.654 1.68c-.364.477-.894 1.007-1.53 1.642l-.208.208a.75.75 0 1 1-1.06-1.06l.171-.172c.682-.682 1.139-1.14 1.434-1.528c.283-.37.347-.586.347-.77s-.064-.4-.347-.77c-.295-.387-.752-.846-1.434-1.528l-.171-.172a.75.75 0 0 1 0-1.06m-7 0a.75.75 0 0 1 1.06 1.06l-.171.172c-.682.682-1.138 1.14-1.434 1.528c-.283.37-.346.586-.346.77s.063.4.346.77c.296.387.752.846 1.434 1.528l.172.172a.75.75 0 1 1-1.061 1.06l-.208-.208c-.636-.635-1.166-1.165-1.53-1.642c-.384-.504-.653-1.036-.653-1.68s.27-1.176.653-1.68c.364-.477.894-1.007 1.53-1.642z"/>',
  // notch glyphs
  gamepad: '<path fill="currentColor" d="m10.667 6.134l-.502-.355A4.24 4.24 0 0 0 7.715 5h-.612c-.405 0-.813.025-1.194.16c-2.383.846-4.022 3.935-3.903 10.943c.024 1.412.354 2.972 1.628 3.581A3.2 3.2 0 0 0 5.027 20a2.74 2.74 0 0 0 1.53-.437c.41-.268.77-.616 1.13-.964c.444-.43.888-.86 1.424-1.138a4.1 4.1 0 0 1 1.89-.461H13c.658 0 1.306.158 1.89.46c.536.279.98.709 1.425 1.139c.36.348.72.696 1.128.964c.39.256.895.437 1.531.437a3.2 3.2 0 0 0 1.393-.316c1.274-.609 1.604-2.17 1.628-3.581c.119-7.008-1.52-10.097-3.903-10.942C17.71 5.025 17.3 5 16.897 5h-.612a4.24 4.24 0 0 0-2.45.78l-.502.354a2.31 2.31 0 0 1-2.666 0" opacity=".5"/><path fill="currentColor" d="M16.75 9a.75.75 0 1 1 0 1.5a.75.75 0 0 1 0-1.5m-9.25.25a.75.75 0 0 1 .75.75v.75H9a.75.75 0 0 1 0 1.5h-.75V13a.75.75 0 0 1-1.5 0v-.75H6a.75.75 0 0 1 0-1.5h.75V10a.75.75 0 0 1 .75-.75m11.5 2a.75.75 0 1 1-1.5 0a.75.75 0 0 1 1.5 0m-3.75.75a.75.75 0 1 0 0-1.5a.75.75 0 0 0 0 1.5m2.25.75a.75.75 0 1 0-1.5 0a.75.75 0 0 0 1.5 0"/>',
  mic: '<g fill="none" stroke="currentColor" stroke-width="1.5"><path d="M17 7v4a5 5 0 0 1-10 0V7a5 5 0 0 1 10 0Z"/><path stroke-linecap="round" d="M17 7h-3m3 4h-3m6 0a8 8 0 0 1-8 8m0 0a8 8 0 0 1-8-8m8 8v3m0 0h3m-3 0H9"/></g>',
  'mic-off': '<path fill="none" stroke="currentColor" stroke-linecap="round" stroke-width="1.5" d="m2 2l20 20M4 11a8 8 0 0 0 8 8m0 0c1.954 0 3.745-.7 5.135-1.865M12 19v3m0 0h3m-3 0H9m11-11c0 1.651-.5 3.186-1.358 4.46m-1.634-8.464c0-2.761-2.239-4.98-5-4.98c-1.869 0-3.47.965-4.328 2.484m9.328 2.496l-3.028.012m3.028-.012v4.008m-10-4.008v4.02a5 5 0 0 0 5 5c1.135 0 2.165-.39 3.004-1.028m1.435-1.728c.358-.69.56-1.413.56-2.244v-.012m-2.824 0h2.825"/>',
  // actions
  plus: '<path fill="currentColor" d="M22 12c0 5.523-4.477 10-10 10S2 17.523 2 12S6.477 2 12 2s10 4.477 10 10" opacity=".5"/><path fill="currentColor" d="M12.75 9a.75.75 0 0 0-1.5 0v2.25H9a.75.75 0 0 0 0 1.5h2.25V15a.75.75 0 0 0 1.5 0v-2.25H15a.75.75 0 0 0 0-1.5h-2.25z"/>',
  x: '<path fill="currentColor" d="M22 12c0 5.523-4.477 10-10 10S2 17.523 2 12S6.477 2 12 2s10 4.477 10 10" opacity=".5"/><path fill="currentColor" d="M8.97 8.97a.75.75 0 0 1 1.06 0L12 10.94l1.97-1.97a.75.75 0 1 1 1.06 1.06L13.06 12l1.97 1.97a.75.75 0 0 1-1.06 1.06L12 13.06l-1.97 1.97a.75.75 0 0 1-1.06-1.06L10.94 12l-1.97-1.97a.75.75 0 0 1 0-1.06"/>',
  edit: '<path fill="currentColor" fill-rule="evenodd" d="M3.25 22a.75.75 0 0 1 .75-.75h16a.75.75 0 0 1 0 1.5H4a.75.75 0 0 1-.75-.75" clip-rule="evenodd" opacity=".5"/><path fill="currentColor" d="M19.08 7.372a3.147 3.147 0 0 0-4.45-4.45l-.71.71l.031.089c.26.75.751 1.733 1.675 2.656a7 7 0 0 0 2.745 1.705z" opacity=".5"/><path fill="currentColor" d="m13.951 3.6l-.03.03l.03.09c.26.75.75 1.732 1.674 2.656A7 7 0 0 0 18.37 8.08l-6.85 6.85c-.462.462-.693.693-.948.891q-.452.352-.969.6c-.291.138-.601.241-1.22.448l-3.268 1.09a.849.849 0 0 1-1.073-1.074l1.089-3.268c.206-.62.31-.93.448-1.22q.247-.518.6-.97c.198-.254.429-.485.89-.947z"/>',
  trash: '<path fill="currentColor" d="M3 6.386c0-.484.345-.877.771-.877h2.665c.529-.016.996-.399 1.176-.965l.03-.1l.115-.391c.07-.24.131-.45.217-.637c.338-.739.964-1.252 1.687-1.383c.184-.033.378-.033.6-.033h3.478c.223 0 .417 0 .6.033c.723.131 1.35.644 1.687 1.383c.086.187.147.396.218.637l.114.391l.03.1c.18.566.74.95 1.27.965h2.57c.427 0 .772.393.772.877s-.345.877-.771.877H3.77c-.425 0-.77-.393-.77-.877"/><path fill="currentColor" fill-rule="evenodd" d="M9.425 11.482c.413-.044.78.273.821.707l.5 5.263c.041.433-.26.82-.671.864c-.412.043-.78-.273-.821-.707l-.5-5.263c-.041-.434.26-.821.671-.864m5.15 0c.412.043.713.43.671.864l-.5 5.263c-.04.434-.408.75-.82.707c-.413-.044-.713-.43-.672-.864l.5-5.264c.041-.433.409-.75.82-.707" clip-rule="evenodd"/><path fill="currentColor" d="M11.596 22h.808c2.783 0 4.174 0 5.08-.886c.904-.886.996-2.339 1.181-5.245l.267-4.188c.1-1.577.15-2.366-.303-2.865c-.454-.5-1.22-.5-2.753-.5H8.124c-1.533 0-2.3 0-2.753.5s-.404 1.288-.303 2.865l.267 4.188c.185 2.906.277 4.36 1.182 5.245c.905.886 2.296.886 5.079.886" opacity=".5"/>',
  search: '<path fill="currentColor" d="M20.313 11.157a9.157 9.157 0 1 1-18.313 0a9.157 9.157 0 0 1 18.313 0" opacity=".5"/><path fill="currentColor" d="m17.1 18.122l3.666 3.666a.723.723 0 0 0 1.023-1.022L18.122 17.1a9 9 0 0 1-1.022 1.022"/>',
  settings: '<path fill="currentColor" fill-rule="evenodd" d="M14.279 2.152C13.909 2 13.439 2 12.5 2s-1.408 0-1.779.152a2 2 0 0 0-1.09 1.083c-.094.223-.13.484-.145.863a1.62 1.62 0 0 1-.796 1.353a1.64 1.64 0 0 1-1.579.008c-.338-.178-.583-.276-.825-.308a2.03 2.03 0 0 0-1.49.396c-.318.242-.553.646-1.022 1.453c-.47.807-.704 1.21-.757 1.605c-.07.526.074 1.058.4 1.479c.148.192.357.353.68.555c.477.297.783.803.783 1.361s-.306 1.064-.782 1.36c-.324.203-.533.364-.682.556a2 2 0 0 0-.399 1.479c.053.394.287.798.757 1.605s.704 1.21 1.022 1.453c.424.323.96.465 1.49.396c.242-.032.487-.13.825-.308a1.64 1.64 0 0 1 1.58.008c.486.28.774.795.795 1.353c.015.38.051.64.145.863c.204.49.596.88 1.09 1.083c.37.152.84.152 1.779.152s1.409 0 1.779-.152a2 2 0 0 0 1.09-1.083c.094-.223.13-.483.145-.863c.02-.558.309-1.074.796-1.353a1.64 1.64 0 0 1 1.579-.008c.338.178.583.276.825.308c.53.07 1.066-.073 1.49-.396c.318-.242.553-.646 1.022-1.453c.47-.807.704-1.21.757-1.605a2 2 0 0 0-.4-1.479c-.148-.192-.357-.353-.68-.555c-.477-.297-.783-.803-.783-1.361s.306-1.064.782-1.36c.324-.203.533-.364.682-.556a2 2 0 0 0 .399-1.479c-.053-.394-.287-.798-.757-1.605s-.704-1.21-1.022-1.453a2.03 2.03 0 0 0-1.49-.396c-.242.032-.487.13-.825.308a1.64 1.64 0 0 1-1.58-.008a1.62 1.62 0 0 1-.795-1.353c-.015-.38-.051-.64-.145-.863a2 2 0 0 0-1.09-1.083" clip-rule="evenodd" opacity=".5"/><path fill="currentColor" d="M15.523 12c0 1.657-1.354 3-3.023 3s-3.023-1.343-3.023-3S10.83 9 12.5 9s3.023 1.343 3.023 3"/>',
  sun: '<path fill="currentColor" d="M18 12a6 6 0 1 1-12 0a6 6 0 0 1 12 0"/><path fill="currentColor" fill-rule="evenodd" d="M12 1.25a.75.75 0 0 1 .75.75v1a.75.75 0 0 1-1.5 0V2a.75.75 0 0 1 .75-.75M1.25 12a.75.75 0 0 1 .75-.75h1a.75.75 0 0 1 0 1.5H2a.75.75 0 0 1-.75-.75m19 0a.75.75 0 0 1 .75-.75h1a.75.75 0 0 1 0 1.5h-1a.75.75 0 0 1-.75-.75M12 20.25a.75.75 0 0 1 .75.75v1a.75.75 0 0 1-1.5 0v-1a.75.75 0 0 1 .75-.75" clip-rule="evenodd"/><path fill="currentColor" d="M4.398 4.398a.75.75 0 0 1 1.061 0l.393.393a.75.75 0 0 1-1.06 1.06l-.394-.392a.75.75 0 0 1 0-1.06m15.202 0a.75.75 0 0 1 0 1.06l-.392.393a.75.75 0 0 1-1.06-1.06l.392-.393a.75.75 0 0 1 1.06 0m-1.453 13.748a.75.75 0 0 1 1.061 0l.393.393a.75.75 0 0 1-1.06 1.06l-.394-.392a.75.75 0 0 1 0-1.06m-12.295 0a.75.75 0 0 1 0 1.06l-.393.393a.75.75 0 1 1-1.06-1.06l.392-.393a.75.75 0 0 1 1.06 0" opacity=".5"/>',
  moon: '<path fill="currentColor" fill-rule="evenodd" d="M22 12c0 5.523-4.477 10-10 10a10 10 0 0 1-3.321-.564A9 9 0 0 1 8 18a8.97 8.97 0 0 1 2.138-5.824A6.5 6.5 0 0 0 15.5 15a6.5 6.5 0 0 0 5.567-3.143c.24-.396.933-.32.933.143" clip-rule="evenodd" opacity=".5"/><path fill="currentColor" d="M2 12c0 4.359 2.789 8.066 6.679 9.435A9 9 0 0 1 8 18c0-2.221.805-4.254 2.138-5.824A6.47 6.47 0 0 1 9 8.5a6.5 6.5 0 0 1 3.143-5.567C12.54 2.693 12.463 2 12 2C6.477 2 2 6.477 2 12"/>',
  'chevron-down': '<path fill="currentColor" d="m8.303 12.404l3.327 3.431c.213.22.527.22.74 0l6.43-6.63C19.201 8.79 18.958 8 18.43 8h-5.723z"/><path fill="currentColor" d="M11.293 8H5.57c-.528 0-.771.79-.37 1.205l2.406 2.481z" opacity=".5"/>',
  check: '<path fill="currentColor" d="M3.464 20.536C4.93 22 7.286 22 12 22s7.071 0 8.535-1.465C22 19.072 22 16.714 22 12s0-7.071-1.465-8.536C19.072 2 16.714 2 12 2S4.929 2 3.464 3.464C2 4.93 2 7.286 2 12s0 7.071 1.464 8.535" opacity=".5"/><path fill="currentColor" d="M18.581 9.474a.75.75 0 1 0-1.162-.948l-5.168 6.33a.749.749 0 0 0-.879 1.116l.286.438a.75.75 0 0 0 1.209.064zm-4 0a.75.75 0 1 0-1.162-.948l-5.133 6.288l-1.705-2.088a.75.75 0 0 0-1.162.948l2.286 2.8a.75.75 0 0 0 1.162 0z"/>',
  // bare tick, lifted from Solar's own check-square-bold-duotone inner path
  tick: '<path fill="currentColor" d="M14.581 9.474a.75.75 0 1 0-1.162-.948l-5.133 6.288l-1.705-2.088a.75.75 0 0 0-1.162.948l2.286 2.8a.75.75 0 0 0 1.162 0z"/>',
  notification: '<path fill="currentColor" d="M18.75 9v.704c0 .845.24 1.671.692 2.374l1.108 1.723c1.011 1.574.239 3.713-1.52 4.21a25.8 25.8 0 0 1-14.06 0c-1.759-.497-2.531-2.636-1.52-4.21l1.108-1.723a4.4 4.4 0 0 0 .693-2.374V9c0-3.866 3.022-7 6.749-7s6.75 3.134 6.75 7" opacity=".5"/><path fill="currentColor" d="M7.243 18.545a5.002 5.002 0 0 0 9.513 0c-3.145.59-6.367.59-9.513 0"/>',
  play: '<path fill="currentColor" fill-rule="evenodd" d="M23 12c0-1.035-.53-2.07-1.591-2.647L8.597 2.385C6.534 1.264 4 2.724 4 5.033V12z" clip-rule="evenodd"/><path fill="currentColor" d="m8.597 21.615l12.812-6.968A2.99 2.99 0 0 0 23 12H4v6.967c0 2.31 2.534 3.769 4.597 2.648" opacity=".5"/>',
  external: '<path fill="currentColor" fill-rule="evenodd" d="M17.47 15.53a.75.75 0 0 0 1.28-.53V6a.75.75 0 0 0-.75-.75H9a.75.75 0 0 0-.53 1.28z" clip-rule="evenodd"/><path fill="currentColor" d="M5.47 17.47a.75.75 0 1 0 1.06 1.06l6.97-6.97l-1.06-1.06z" opacity=".5"/>',
  pin: '<path fill="currentColor" fill-rule="evenodd" d="m16.219 4.838l2.964 2.967c2.012 2.014 3.018 3.021 2.784 4.107c-.235 1.085-1.567 1.585-4.23 2.586l-1.845.693c-.713.268-1.07.402-1.345.64q-.181.158-.322.352c-.212.297-.313.664-.515 1.4c-.46 1.672-.69 2.508-1.239 2.821c-.23.132-.492.2-.758.2c-.63 0-1.243-.614-2.469-1.84l-1.466-1.468l-1.079-1.08L5.285 14.8c-1.218-1.219-1.827-1.828-1.83-2.455a1.53 1.53 0 0 1 .203-.773c.313-.543 1.143-.772 2.803-1.23c.737-.203 1.105-.304 1.402-.517q.199-.144.36-.332c.236-.278.368-.637.63-1.355l.669-1.823c.987-2.693 1.48-4.04 2.568-4.28s2.102.774 4.129 2.803" clip-rule="evenodd" opacity=".5"/><path fill="currentColor" d="m3.302 21.776l4.476-4.48l-1.079-1.08l-4.476 4.48a.764.764 0 0 0 1.08 1.08"/>'
};
function Icon({
  name = 'app',
  size = 20,
  strokeWidth,
  color = 'currentColor',
  style,
  ...rest
}) {
  let inner = PATHS[name];
  if (!inner) {
    // Loud, not silent: a typo'd name used to fall through to the app glyph,
    // which renders a plausible-looking wrong icon with no error.
    console.warn(`[Cortinhos] Icon: unknown name "${name}". Valid: ${Object.keys(PATHS).join(', ')}`);
    return null;
  }
  // Only the stroke-based mic pair responds to strokeWidth.
  if (strokeWidth) inner = inner.replace(/stroke-width="[\d.]+"/g, `stroke-width="${strokeWidth}"`);
  return /*#__PURE__*/React.createElement("svg", _extends({
    width: size,
    height: size,
    viewBox: "0 0 24 24",
    style: {
      display: 'block',
      flexShrink: 0,
      color,
      ...style
    },
    "aria-hidden": "true",
    dangerouslySetInnerHTML: {
      __html: inner
    }
  }, rest));
}
Object.assign(__ds_scope, { Icon });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/core/Icon.jsx", error: String((e && e.message) || e) }); }

// components/core/ShortcutTile.jsx
try { (() => {
const TYPE_META = {
  app: {
    icon: 'app',
    label: 'App',
    grad: 'linear-gradient(150deg,#1f4a73,#132b45)',
    glow: '#3ea6ff'
  },
  folder: {
    icon: 'folder',
    label: 'Pasta',
    grad: 'linear-gradient(150deg,#6a4a12,#3a2a10)',
    glow: '#f0a92c'
  },
  url: {
    icon: 'url',
    label: 'Link',
    grad: 'linear-gradient(150deg,#12564a,#0f3330)',
    glow: '#22c55e'
  },
  command: {
    icon: 'command',
    label: 'Comando',
    grad: 'linear-gradient(150deg,#3d2f5e,#241a38)',
    glow: '#a78bfa'
  }
};

/**
 * Steam-library-style shortcut capsule: full-bleed cover art (or a typed
 * gradient + glyph watermark when no image), the name in a dark bottom bar.
 * Resting → hover lifts + brightens + reveals the "Abrir" action → pressed dips.
 */
function ShortcutTile({
  name,
  type = 'app',
  cover,
  hotkey,
  running = false,
  pinned = false,
  onOpen,
  onContextMenu,
  actionLabel = 'Abrir',
  style
}) {
  const [hover, setHover] = React.useState(false);
  const [active, setActive] = React.useState(false);
  const meta = TYPE_META[type] || TYPE_META.app;
  return /*#__PURE__*/React.createElement("div", {
    onMouseEnter: () => setHover(true),
    onMouseLeave: () => {
      setHover(false);
      setActive(false);
    },
    onMouseDown: () => setActive(true),
    onMouseUp: () => setActive(false),
    onClick: onOpen,
    onContextMenu: onContextMenu,
    style: {
      position: 'relative',
      display: 'block',
      aspectRatio: '3 / 4',
      overflow: 'hidden',
      borderRadius: 'var(--radius-md)',
      border: '1px solid var(--border-subtle)',
      cursor: 'pointer',
      userSelect: 'none',
      boxShadow: hover ? 'var(--elev-3)' : 'var(--elev-1)',
      outline: hover ? '2px solid var(--accent)' : '2px solid transparent',
      outlineOffset: -2,
      transform: active ? 'translateY(0) scale(.985)' : hover ? 'translateY(-3px)' : 'translateY(0)',
      transition: 'transform var(--dur-med) var(--ease-spring), box-shadow var(--dur-med) var(--ease-out), outline-color var(--dur-fast)',
      ...style
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      background: cover ? `#0e1116 url("${cover}") center/cover no-repeat` : meta.grad,
      filter: hover ? 'brightness(1.08)' : 'none',
      transition: 'filter var(--dur-med) var(--ease-out)'
    }
  }, !cover && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      opacity: .9
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 60,
      height: 60,
      borderRadius: 'var(--radius-lg)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      background: 'rgba(255,255,255,.07)',
      border: '1px solid rgba(255,255,255,.12)',
      boxShadow: `0 0 40px ${meta.glow}55`
    }
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: meta.icon,
    size: 30,
    color: "#fff"
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      right: -6,
      bottom: 26,
      fontFamily: 'var(--font-display)',
      fontWeight: 700,
      fontSize: 96,
      lineHeight: 1,
      color: 'rgba(255,255,255,.05)',
      pointerEvents: 'none'
    }
  }, (name || '?').charAt(0).toUpperCase()))), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      top: 10,
      left: 10,
      right: 10,
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'flex-start',
      pointerEvents: 'none'
    }
  }, running ? /*#__PURE__*/React.createElement(__ds_scope.Badge, {
    tone: "success",
    icon: /*#__PURE__*/React.createElement(__ds_scope.Icon, {
      name: "gamepad",
      size: 11
    })
  }, "rodando") : /*#__PURE__*/React.createElement("span", null), pinned && /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: "pin",
    size: 15,
    color: "#fff",
    style: {
      filter: 'drop-shadow(0 1px 2px rgba(0,0,0,.6))'
    }
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
      padding: '20px 12px 11px',
      background: 'linear-gradient(to top, rgba(8,10,13,.94) 55%, rgba(8,10,13,0))'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-display)',
      fontWeight: 600,
      fontSize: 'var(--fs-subtitle)',
      color: '#fff',
      lineHeight: 1.15,
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap',
      textShadow: '0 1px 3px rgba(0,0,0,.6)'
    }
  }, name), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6,
      marginTop: 6
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 600,
      color: 'rgba(255,255,255,.6)'
    }
  }, meta.label), hotkey && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement("span", {
    style: {
      color: 'rgba(255,255,255,.3)'
    }
  }, "\xB7"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--accent)'
    }
  }, hotkey)))), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
      padding: 12,
      display: 'flex',
      justifyContent: 'center',
      opacity: hover ? 1 : 0,
      transform: hover ? 'translateY(0)' : 'translateY(8px)',
      transition: 'opacity var(--dur-med) var(--ease-out), transform var(--dur-med) var(--ease-spring)',
      pointerEvents: 'none'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 6,
      padding: '8px 18px',
      borderRadius: 'var(--radius-md)',
      background: active ? 'var(--accent-press)' : 'var(--accent)',
      color: 'var(--text-on-accent)',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      fontWeight: 'var(--fw-semibold)',
      boxShadow: 'var(--elev-2)'
    }
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: "play",
    size: 14
  }), actionLabel)));
}
Object.assign(__ds_scope, { ShortcutTile });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/core/ShortcutTile.jsx", error: String((e && e.message) || e) }); }

// components/feedback/ContextMenu.jsx
try { (() => {
/** Right-click menu. Render conditionally at {x,y}; items: {label, icon, danger, onClick}. */
function ContextMenu({
  x = 0,
  y = 0,
  items = [],
  onClose,
  open = true
}) {
  const ref = React.useRef(null);
  React.useEffect(() => {
    if (!open) return;
    function onDoc(e) {
      if (ref.current && !ref.current.contains(e.target)) onClose && onClose();
    }
    function onKey(e) {
      if (e.key === 'Escape') onClose && onClose();
    }
    document.addEventListener('mousedown', onDoc);
    document.addEventListener('keydown', onKey);
    return () => {
      document.removeEventListener('mousedown', onDoc);
      document.removeEventListener('keydown', onKey);
    };
  }, [open, onClose]);
  if (!open) return null;
  return /*#__PURE__*/React.createElement("div", {
    ref: ref,
    role: "menu",
    style: {
      position: 'fixed',
      top: y,
      left: x,
      zIndex: 1000,
      minWidth: 168,
      padding: 4,
      background: 'var(--surface-3)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-md)',
      boxShadow: 'var(--elev-4)',
      animation: 'ctx-in var(--dur-fast) var(--ease-out)'
    }
  }, /*#__PURE__*/React.createElement("style", null, `@keyframes ctx-in{from{opacity:0;transform:scale(.96) translateY(-4px)}to{opacity:1;transform:none}}`), items.map((it, i) => it.divider ? /*#__PURE__*/React.createElement("div", {
    key: i,
    style: {
      height: 1,
      background: 'var(--border-subtle)',
      margin: '4px 0'
    }
  }) : /*#__PURE__*/React.createElement("button", {
    key: i,
    type: "button",
    role: "menuitem",
    onClick: () => {
      it.onClick && it.onClick();
      onClose && onClose();
    },
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      width: '100%',
      padding: '7px 10px',
      border: 'none',
      background: 'transparent',
      cursor: 'pointer',
      borderRadius: 'var(--radius-sm)',
      textAlign: 'left',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      color: it.danger ? 'var(--danger)' : 'var(--text-primary)'
    },
    onMouseEnter: e => e.currentTarget.style.background = it.danger ? 'var(--danger-soft)' : 'var(--hover-overlay)',
    onMouseLeave: e => e.currentTarget.style.background = 'transparent'
  }, it.icon, /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1
    }
  }, it.label))));
}
Object.assign(__ds_scope, { ContextMenu });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/feedback/ContextMenu.jsx", error: String((e && e.message) || e) }); }

// components/feedback/Dialog.jsx
try { (() => {
/** Modal dialog with scrim, Mica-ish surface, header + close, body, and optional footer. */
function Dialog({
  open = true,
  title,
  children,
  footer,
  onClose,
  width = 440
}) {
  React.useEffect(() => {
    if (!open) return;
    function onKey(e) {
      if (e.key === 'Escape') onClose && onClose();
    }
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [open, onClose]);
  if (!open) return null;
  return /*#__PURE__*/React.createElement("div", {
    onMouseDown: e => {
      if (e.target === e.currentTarget) onClose && onClose();
    },
    style: {
      position: 'fixed',
      inset: 0,
      zIndex: 900,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      padding: 24,
      background: 'var(--scrim)',
      backdropFilter: 'blur(2px)',
      animation: 'dlg-scrim var(--dur-fast) var(--ease-out)'
    }
  }, /*#__PURE__*/React.createElement("style", null, `@keyframes dlg-scrim{from{opacity:0}to{opacity:1}}@keyframes dlg-pop{from{opacity:0;transform:scale(.96) translateY(8px)}to{opacity:1;transform:none}}`), /*#__PURE__*/React.createElement("div", {
    role: "dialog",
    "aria-modal": "true",
    style: {
      width,
      maxWidth: '100%',
      maxHeight: '90vh',
      display: 'flex',
      flexDirection: 'column',
      background: 'var(--surface-1)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-lg)',
      boxShadow: 'var(--elev-4)',
      overflow: 'hidden',
      animation: 'dlg-pop var(--dur-med) var(--ease-spring)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      gap: 12,
      padding: '16px 18px',
      borderBottom: '1px solid var(--border-subtle)'
    }
  }, /*#__PURE__*/React.createElement("h2", {
    style: {
      margin: 0,
      fontFamily: 'var(--font-display)',
      fontWeight: 'var(--fw-semibold)',
      fontSize: 'var(--fs-title)',
      color: 'var(--text-primary)'
    }
  }, title), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: onClose,
    "aria-label": "Fechar",
    style: {
      display: 'flex',
      width: 30,
      height: 30,
      alignItems: 'center',
      justifyContent: 'center',
      border: 'none',
      background: 'transparent',
      borderRadius: 'var(--radius-sm)',
      color: 'var(--text-tertiary)',
      cursor: 'pointer'
    },
    onMouseEnter: e => e.currentTarget.style.background = 'var(--hover-overlay)',
    onMouseLeave: e => e.currentTarget.style.background = 'transparent'
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: "x",
    size: 18
  }))), /*#__PURE__*/React.createElement("div", {
    style: {
      padding: 18,
      overflowY: 'auto',
      display: 'flex',
      flexDirection: 'column',
      gap: 14
    }
  }, children), footer && /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      justifyContent: 'flex-end',
      gap: 10,
      padding: '14px 18px',
      borderTop: '1px solid var(--border-subtle)',
      background: 'var(--surface-1)'
    }
  }, footer)));
}
Object.assign(__ds_scope, { Dialog });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/feedback/Dialog.jsx", error: String((e && e.message) || e) }); }

// components/feedback/Tooltip.jsx
try { (() => {
/** Lightweight hover tooltip. Wraps its children; shows `label` on hover after a short delay. */
function Tooltip({
  label,
  children,
  placement = 'top',
  style
}) {
  const [show, setShow] = React.useState(false);
  const timer = React.useRef(null);
  const pos = {
    top: {
      bottom: '100%',
      left: '50%',
      transform: 'translate(-50%,-6px)'
    },
    bottom: {
      top: '100%',
      left: '50%',
      transform: 'translate(-50%,6px)'
    },
    right: {
      left: '100%',
      top: '50%',
      transform: 'translate(6px,-50%)'
    },
    left: {
      right: '100%',
      top: '50%',
      transform: 'translate(-6px,-50%)'
    }
  }[placement];
  return /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'relative',
      display: 'inline-flex',
      ...style
    },
    onMouseEnter: () => {
      timer.current = setTimeout(() => setShow(true), 350);
    },
    onMouseLeave: () => {
      clearTimeout(timer.current);
      setShow(false);
    }
  }, children, show && /*#__PURE__*/React.createElement("span", {
    role: "tooltip",
    style: {
      position: 'absolute',
      zIndex: 1200,
      ...pos,
      whiteSpace: 'nowrap',
      pointerEvents: 'none',
      padding: '5px 9px',
      borderRadius: 'var(--radius-sm)',
      background: 'var(--surface-3)',
      color: 'var(--text-primary)',
      border: '1px solid var(--border-subtle)',
      boxShadow: 'var(--elev-2)',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-medium)',
      animation: 'tip-in var(--dur-fast) var(--ease-out)'
    }
  }, /*#__PURE__*/React.createElement("style", null, `@keyframes tip-in{from{opacity:0}to{opacity:1}}`), label));
}
Object.assign(__ds_scope, { Tooltip });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/feedback/Tooltip.jsx", error: String((e && e.message) || e) }); }

// components/forms/Button.jsx
try { (() => {
function _extends() { return _extends = Object.assign ? Object.assign.bind() : function (n) { for (var e = 1; e < arguments.length; e++) { var t = arguments[e]; for (var r in t) ({}).hasOwnProperty.call(t, r) && (n[r] = t[r]); } return n; }, _extends.apply(null, arguments); }
const SIZES = {
  sm: {
    height: 28,
    padding: '0 10px',
    font: 'var(--fs-caption)',
    gap: 6,
    icon: 15
  },
  md: {
    height: 34,
    padding: '0 14px',
    font: 'var(--fs-body)',
    gap: 7,
    icon: 17
  },
  lg: {
    height: 40,
    padding: '0 18px',
    font: 'var(--fs-subtitle)',
    gap: 8,
    icon: 19
  }
};
function variantStyle(variant, hover, active) {
  switch (variant) {
    case 'secondary':
      return {
        backgroundColor: active ? 'var(--surface-3)' : hover ? 'var(--surface-2)' : 'var(--surface-1)',
        color: 'var(--text-primary)',
        border: '1px solid var(--border-strong)'
      };
    case 'ghost':
      return {
        backgroundColor: active ? 'var(--press-overlay)' : hover ? 'var(--hover-overlay)' : 'transparent',
        color: 'var(--text-secondary)',
        border: '1px solid transparent'
      };
    case 'danger':
      return {
        backgroundColor: active ? 'var(--danger)' : hover ? 'var(--danger)' : 'var(--danger-soft)',
        color: hover || active ? '#fff' : 'var(--danger)',
        border: '1px solid transparent'
      };
    default:
      // primary
      return {
        backgroundColor: active ? 'var(--accent-press)' : hover ? 'var(--accent-hover)' : 'var(--accent)',
        color: 'var(--text-on-accent)',
        border: '1px solid transparent'
      };
  }
}
function Button({
  children,
  variant = 'primary',
  size = 'md',
  icon,
  iconRight,
  iconOnly = false,
  disabled = false,
  style,
  ...rest
}) {
  const [hover, setHover] = React.useState(false);
  const [active, setActive] = React.useState(false);
  const s = SIZES[size] || SIZES.md;
  const vs = variantStyle(variant, hover && !disabled, active && !disabled);
  return /*#__PURE__*/React.createElement("button", _extends({
    type: "button",
    disabled: disabled,
    onMouseEnter: () => setHover(true),
    onMouseLeave: () => {
      setHover(false);
      setActive(false);
    },
    onMouseDown: () => setActive(true),
    onMouseUp: () => setActive(false),
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: s.gap,
      height: s.height,
      padding: iconOnly ? 0 : s.padding,
      width: iconOnly ? s.height : undefined,
      fontFamily: 'var(--font-body)',
      fontSize: s.font,
      fontWeight: 'var(--fw-semibold)',
      borderRadius: 'var(--radius-md)',
      cursor: disabled ? 'not-allowed' : 'pointer',
      opacity: disabled ? 0.45 : 1,
      transform: active && !disabled ? 'scale(.97)' : 'scale(1)',
      transition: 'background-color var(--dur-fast) var(--ease-out), transform var(--dur-fast) var(--ease-spring), color var(--dur-fast)',
      whiteSpace: 'nowrap',
      outline: 'none',
      ...vs,
      ...style
    }
  }, rest), icon, !iconOnly && children, iconRight);
}
Object.assign(__ds_scope, { Button });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/forms/Button.jsx", error: String((e && e.message) || e) }); }

// components/forms/Checkbox.jsx
try { (() => {
function Checkbox({
  checked = false,
  onChange,
  label,
  disabled = false,
  style
}) {
  return /*#__PURE__*/React.createElement("label", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 9,
      cursor: disabled ? 'not-allowed' : 'pointer',
      opacity: disabled ? 0.5 : 1,
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      color: 'var(--text-primary)',
      ...style
    }
  }, /*#__PURE__*/React.createElement("span", {
    onClick: () => !disabled && onChange && onChange(!checked),
    style: {
      width: 18,
      height: 18,
      borderRadius: 5,
      flexShrink: 0,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      backgroundColor: checked ? 'var(--accent)' : 'var(--surface-2)',
      border: `1px solid ${checked ? 'var(--accent)' : 'var(--border-strong)'}`,
      transition: 'background-color var(--dur-fast), border-color var(--dur-fast)'
    }
  }, checked && /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: "tick",
    size: 15,
    color: "var(--text-on-accent)"
  })), label);
}
Object.assign(__ds_scope, { Checkbox });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/forms/Checkbox.jsx", error: String((e && e.message) || e) }); }

// components/forms/Select.jsx
try { (() => {
function Select({
  label,
  value,
  options = [],
  onChange,
  disabled = false,
  style
}) {
  const [open, setOpen] = React.useState(false);
  const [focus, setFocus] = React.useState(false);
  const ref = React.useRef(null);
  const current = options.find(o => (o.value ?? o) === value);
  const currentLabel = current ? current.label ?? current : null;
  React.useEffect(() => {
    function onDoc(e) {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false);
    }
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, []);
  return /*#__PURE__*/React.createElement("label", {
    ref: ref,
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 6,
      position: 'relative',
      ...style
    }
  }, label && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-semibold)',
      color: 'var(--text-secondary)',
      letterSpacing: 'var(--tracking-caps)',
      textTransform: 'uppercase'
    }
  }, label), /*#__PURE__*/React.createElement("button", {
    type: "button",
    disabled: disabled,
    onClick: () => setOpen(o => !o),
    onFocus: () => setFocus(true),
    onBlur: () => setFocus(false),
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      gap: 8,
      height: 36,
      padding: '0 10px 0 12px',
      background: 'var(--surface-2)',
      borderRadius: 'var(--radius-sm)',
      border: `1px solid ${open || focus ? 'var(--accent)' : 'var(--border-subtle)'}`,
      boxShadow: open || focus ? 'var(--ring)' : 'none',
      color: currentLabel ? 'var(--text-primary)' : 'var(--text-tertiary)',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      cursor: disabled ? 'not-allowed' : 'pointer',
      opacity: disabled ? 0.5 : 1,
      outline: 'none',
      transition: 'border-color var(--dur-fast), box-shadow var(--dur-fast)'
    }
  }, /*#__PURE__*/React.createElement("span", null, currentLabel || 'Selecione…'), /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: "chevron-down",
    size: 16,
    color: "var(--text-tertiary)",
    style: {
      transform: open ? 'rotate(180deg)' : 'none',
      transition: 'transform var(--dur-fast) var(--ease-out)'
    }
  })), open && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      top: 'calc(100% + 4px)',
      left: 0,
      right: 0,
      zIndex: 20,
      background: 'var(--surface-3)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-md)',
      boxShadow: 'var(--elev-4)',
      padding: 4
    }
  }, options.map(o => {
    const val = o.value ?? o,
      lbl = o.label ?? o;
    const sel = val === value;
    return /*#__PURE__*/React.createElement("div", {
      key: val,
      onClick: () => {
        onChange && onChange(val);
        setOpen(false);
      },
      style: {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        gap: 8,
        padding: '7px 10px',
        borderRadius: 'var(--radius-sm)',
        cursor: 'pointer',
        fontFamily: 'var(--font-body)',
        fontSize: 'var(--fs-body)',
        color: 'var(--text-primary)',
        background: sel ? 'var(--accent-soft)' : 'transparent'
      },
      onMouseEnter: e => {
        if (!sel) e.currentTarget.style.background = 'var(--hover-overlay)';
      },
      onMouseLeave: e => {
        if (!sel) e.currentTarget.style.background = 'transparent';
      }
    }, o.icon, /*#__PURE__*/React.createElement("span", {
      style: {
        flex: 1
      }
    }, lbl), sel && /*#__PURE__*/React.createElement(__ds_scope.Icon, {
      name: "tick",
      size: 16,
      color: "var(--accent)"
    }));
  })));
}
Object.assign(__ds_scope, { Select });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/forms/Select.jsx", error: String((e && e.message) || e) }); }

// components/forms/TextField.jsx
try { (() => {
function _extends() { return _extends = Object.assign ? Object.assign.bind() : function (n) { for (var e = 1; e < arguments.length; e++) { var t = arguments[e]; for (var r in t) ({}).hasOwnProperty.call(t, r) && (n[r] = t[r]); } return n; }, _extends.apply(null, arguments); }
function TextField({
  label,
  value,
  defaultValue,
  placeholder,
  onChange,
  icon,
  trailing,
  error,
  hint,
  disabled = false,
  style,
  ...rest
}) {
  const [focus, setFocus] = React.useState(false);
  return /*#__PURE__*/React.createElement("label", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 6,
      ...style
    }
  }, label && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-semibold)',
      color: 'var(--text-secondary)',
      letterSpacing: 'var(--tracking-caps)',
      textTransform: 'uppercase'
    }
  }, label), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      height: 36,
      padding: '0 12px',
      background: 'var(--surface-2)',
      borderRadius: 'var(--radius-sm)',
      border: `1px solid ${error ? 'var(--danger)' : focus ? 'var(--accent)' : 'var(--border-subtle)'}`,
      boxShadow: focus && !error ? 'var(--ring)' : 'none',
      transition: 'border-color var(--dur-fast), box-shadow var(--dur-fast)',
      opacity: disabled ? 0.5 : 1
    }
  }, icon && /*#__PURE__*/React.createElement("span", {
    style: {
      color: 'var(--text-tertiary)',
      display: 'flex'
    }
  }, icon), /*#__PURE__*/React.createElement("input", _extends({
    value: value,
    defaultValue: defaultValue,
    placeholder: placeholder,
    onChange: onChange,
    disabled: disabled,
    onFocus: () => setFocus(true),
    onBlur: () => setFocus(false),
    style: {
      flex: 1,
      minWidth: 0,
      background: 'transparent',
      border: 'none',
      outline: 'none',
      color: 'var(--text-primary)',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)'
    }
  }, rest)), trailing), (error || hint) && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      color: error ? 'var(--danger)' : 'var(--text-tertiary)'
    }
  }, error || hint));
}
Object.assign(__ds_scope, { TextField });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/forms/TextField.jsx", error: String((e && e.message) || e) }); }

// components/forms/ThemeToggle.jsx
try { (() => {
/** Segmented sun/moon theme toggle. Controlled: pass theme ('dark'|'light') + onChange. */
function ThemeToggle({
  theme = 'dark',
  onChange,
  style
}) {
  const opts = [{
    v: 'light',
    icon: 'sun'
  }, {
    v: 'dark',
    icon: 'moon'
  }];
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'inline-flex',
      padding: 3,
      gap: 2,
      background: 'var(--surface-2)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-pill)',
      ...style
    }
  }, opts.map(o => {
    const active = theme === o.v;
    return /*#__PURE__*/React.createElement("button", {
      key: o.v,
      type: "button",
      onClick: () => onChange && onChange(o.v),
      "aria-label": o.v,
      style: {
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        width: 28,
        height: 28,
        borderRadius: 'var(--radius-pill)',
        border: 'none',
        cursor: 'pointer',
        backgroundColor: active ? 'var(--accent)' : 'transparent',
        color: active ? 'var(--text-on-accent)' : 'var(--text-tertiary)',
        transition: 'background-color var(--dur-fast) var(--ease-out), color var(--dur-fast)'
      }
    }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
      name: o.icon,
      size: 15
    }));
  }));
}
Object.assign(__ds_scope, { ThemeToggle });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/forms/ThemeToggle.jsx", error: String((e && e.message) || e) }); }

// components/surface/Notch.jsx
try { (() => {
/**
 * Floating always-on-top notch surface — a second, persistent surface of the
 * system, separate from the main window.
 *
 * Size states (how much room it takes):
 *   compact  — idle pill, glyphs only, no text. Smallest footprint.
 *   peek     — hover. Widens, captions appear. Auto-recedes on mouse-out.
 *   expanded — click. Grows into a panel of real controls.
 *
 * Status states (an event overriding the compact pill; priority error > alert
 * > progress). These are transient and independent of the size state:
 *   alert    — short transient message, accent-tinted, auto-recedes.
 *   progress — hairline accent bar hugging the pill's bottom edge.
 *   error    — danger-tinted, does NOT auto-recede; click to dismiss.
 *
 * Plus `dragging` — lifted and scaled while the user repositions it between
 * the three top anchors.
 */
function Notch({
  micMuted = false,
  onToggleMic,
  discordApp = null,
  discordTracking = false,
  onToggleTracking,
  quickShortcuts = [],
  onOpenLauncher,
  hotkeyLabel = 'Ctrl+Alt+L',
  notifications = [],
  onClearNotifications,
  perf = null,
  status = null,
  onDismissStatus,
  anchor = 'top-center',
  dragging = false,
  compactWidth = 92,
  state: controlled,
  onStateChange,
  style
}) {
  const [internal, setInternal] = React.useState('compact');
  const state = controlled ?? internal;
  const setState = s => {
    onStateChange ? onStateChange(s) : setInternal(s);
  };
  const ref = React.useRef(null);
  const timer = React.useRef(null);
  const go = s => {
    clearTimeout(timer.current);
    setState(s);
  };
  const scheduleCollapse = () => {
    clearTimeout(timer.current);
    timer.current = setTimeout(() => setState('compact'), 3200);
  };
  React.useEffect(() => {
    if (state !== 'expanded') return;
    function onDoc(e) {
      if (ref.current && !ref.current.contains(e.target)) go('compact');
    }
    function onKey(e) {
      if (e.key === 'Escape') go('compact');
    }
    document.addEventListener('mousedown', onDoc);
    document.addEventListener('keydown', onKey);
    scheduleCollapse();
    return () => {
      document.removeEventListener('mousedown', onDoc);
      document.removeEventListener('keydown', onKey);
    };
  }, [state]);
  const expanded = state === 'expanded';
  const peek = state === 'peek';
  const kind = status && status.kind;
  const isError = kind === 'error';
  const isAlert = kind === 'alert';
  const isProgress = kind === 'progress';
  // A status label takes over the pill; progress keeps the compact silhouette.
  const showsStatusLabel = !expanded && (isError || isAlert);
  const surface = expanded ? 'var(--material-acrylic)' : isError ? 'var(--danger-soft)' : isAlert ? 'var(--accent-soft)' : 'var(--taskbar-tint)';
  const borderColor = isError ? 'var(--danger)' : 'var(--border-subtle)';
  const minWidth = expanded ? 400 : showsStatusLabel ? 'auto' : peek ? 232 : compactWidth;
  const anchorAlign = {
    'top-left': 'flex-start',
    'top-center': 'center',
    'top-right': 'flex-end'
  }[anchor] || 'center';
  return /*#__PURE__*/React.createElement("div", {
    ref: ref,
    "data-notch-state": state,
    "data-notch-status": kind || 'idle',
    onMouseEnter: () => {
      if (!expanded) go('peek');
    },
    onMouseLeave: () => {
      if (!expanded) go('compact');
    },
    onClick: () => {
      if (isError) {
        onDismissStatus && onDismissStatus();
        return;
      }
      if (!expanded) go('expanded');
    },
    onMouseMove: () => {
      if (expanded) scheduleCollapse();
    },
    style: {
      position: 'relative',
      display: 'flex',
      flexDirection: 'column',
      alignSelf: anchorAlign,
      boxSizing: 'border-box',
      width: expanded ? 400 : 'auto',
      minWidth,
      padding: expanded ? '10px 12px 12px' : '0 8px',
      height: expanded ? 'auto' : 'var(--taskbar-height)',
      backgroundColor: surface,
      backdropFilter: expanded ? 'var(--material-acrylic-blur)' : 'var(--material-acrylic-thin)',
      border: `1px solid ${borderColor}`,
      borderTop: 'none',
      borderRadius: '0 0 var(--radius-notch) var(--radius-notch)',
      boxShadow: dragging ? 'var(--elev-4)' : expanded ? 'var(--elev-notch)'
      // The taskbar's hairline sits on the edge facing the content; for a
      // top-docked surface that is the bottom edge.
      : 'var(--elev-2), inset 0 -1px 0 var(--taskbar-hairline)',
      transform: dragging ? 'scale(1.03)' : 'scale(1)',
      color: 'var(--text-primary)',
      cursor: dragging ? 'grabbing' : expanded ? 'default' : 'pointer',
      userSelect: 'none',
      overflow: isProgress ? 'hidden' : 'visible',
      transition: 'min-width var(--dur-med) var(--ease-spring), width var(--dur-med) var(--ease-spring), background-color var(--dur-med) var(--ease-out), box-shadow var(--dur-med) var(--ease-out), padding var(--dur-med) var(--ease-spring), transform var(--dur-fast) var(--ease-spring)',
      ...style
    }
  }, showsStatusLabel ? /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      height: 'var(--taskbar-height)',
      padding: '0 6px',
      whiteSpace: 'nowrap'
    }
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: status.icon || (isError ? 'x' : 'gamepad'),
    size: 20,
    color: isError ? 'var(--danger)' : 'var(--accent)'
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      fontWeight: 'var(--fw-semibold)',
      color: isError ? 'var(--danger)' : 'var(--text-primary)'
    }
  }, status.label)) : /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 'var(--taskbar-gap)',
      height: 'var(--taskbar-height)',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(Slot, {
    icon: micMuted ? 'mic-off' : 'mic',
    color: micMuted ? 'var(--danger)' : 'var(--text-primary)',
    label: peek || expanded ? micMuted ? 'Mic mutado' : 'Mic ativo' : null,
    labelColor: micMuted ? 'var(--danger)' : 'var(--text-secondary)',
    running: micMuted,
    indicatorColor: "var(--danger)"
  }), (discordTracking || discordApp) && /*#__PURE__*/React.createElement(Slot, {
    icon: "gamepad",
    color: discordTracking ? 'var(--accent)' : 'var(--text-tertiary)',
    label: peek || expanded ? discordApp || 'Discord' : null,
    running: discordTracking,
    active: !!discordApp
  }), notifications.length > 0 && !expanded && /*#__PURE__*/React.createElement(Slot, {
    icon: "notification",
    badge: notifications.length,
    color: "var(--text-primary)",
    label: peek ? notifications.length === 1 ? '1 notificação' : `${notifications.length} notificações` : null
  })), isProgress && !expanded && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: 0,
      right: 0,
      bottom: 0,
      height: 2,
      background: 'var(--surface-3)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: '100%',
      width: `${Math.round((status.progress ?? 0) * 100)}%`,
      background: 'var(--accent)',
      transition: 'width var(--dur-med) var(--ease-out)'
    }
  })), expanded && /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 10,
      marginTop: 8,
      animation: 'notch-body var(--dur-med) var(--ease-out)'
    }
  }, /*#__PURE__*/React.createElement("style", null, `@keyframes notch-body{from{opacity:0;transform:translateY(-6px)}to{opacity:1;transform:none}}`), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 8,
      alignItems: 'flex-start'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0,
      display: 'flex',
      flexDirection: 'column',
      gap: 8
    }
  }, /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: e => {
      e.stopPropagation();
      onToggleMic && onToggleMic();
    },
    style: rowBtn(micMuted)
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: micMuted ? 'mic-off' : 'mic',
    size: 16,
    color: micMuted ? 'var(--danger)' : 'var(--text-primary)'
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      textAlign: 'left'
    }
  }, micMuted ? 'Ativar mic' : 'Mutar mic'), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 'var(--fs-caption)',
      color: micMuted ? 'var(--danger)' : 'var(--text-tertiary)'
    }
  }, micMuted ? 'OFF' : 'ON')), quickShortcuts.length > 0 && /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: labelStyle()
  }, "Atalhos r\xE1pidos"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 6
    }
  }, quickShortcuts.slice(0, 5).map((s, i) => /*#__PURE__*/React.createElement(__ds_scope.Tooltip, {
    key: i,
    label: s.name,
    placement: "bottom"
  }, /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: e => {
      e.stopPropagation();
      s.onOpen && s.onOpen();
    },
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      width: 38,
      height: 38,
      background: 'var(--surface-2)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-sm)',
      cursor: 'pointer',
      color: 'var(--accent)'
    },
    onMouseEnter: e => e.currentTarget.style.background = 'var(--surface-3)',
    onMouseLeave: e => e.currentTarget.style.background = 'var(--surface-2)'
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: s.type || 'app',
    size: 17
  }))))))), notifications.length > 0 && /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      ...labelStyle(),
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'baseline'
    }
  }, /*#__PURE__*/React.createElement("span", null, "Notifica\xE7\xF5es"), /*#__PURE__*/React.createElement("span", {
    onClick: e => {
      e.stopPropagation();
      onClearNotifications && onClearNotifications();
    },
    style: {
      cursor: 'pointer',
      color: 'var(--accent)',
      letterSpacing: 0,
      textTransform: 'none',
      fontWeight: 'var(--fw-medium)'
    }
  }, "Limpar")), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 4
    }
  }, notifications.slice(0, 3).map((n, i) => /*#__PURE__*/React.createElement("div", {
    key: i,
    style: {
      display: 'flex',
      gap: 8,
      padding: '7px 9px',
      background: 'var(--surface-2)',
      borderRadius: 'var(--radius-sm)'
    }
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: n.icon || 'app',
    size: 15,
    color: "var(--accent)",
    style: {
      marginTop: 1
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'flex',
      justifyContent: 'space-between',
      gap: 6
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-semibold)',
      color: 'var(--text-primary)',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap'
    }
  }, n.title), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 9.5,
      color: 'var(--text-tertiary)',
      flexShrink: 0
    }
  }, n.time)), /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'block',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--text-secondary)',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap'
    }
  }, n.body))))))), perf && /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: labelStyle()
  }, "Desempenho"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 6
    }
  }, [['CPU', perf.cpu, '%'], ['GPU', perf.gpu, '%'], ['FPS', perf.fps, '']].map(([k, v, unit]) => /*#__PURE__*/React.createElement("div", {
    key: k,
    style: {
      flex: 1,
      padding: '7px 9px',
      background: 'var(--surface-2)',
      borderRadius: 'var(--radius-sm)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 9.5,
      color: 'var(--text-tertiary)',
      letterSpacing: '.06em'
    }
  }, k), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-display)',
      fontSize: 15,
      fontWeight: 'var(--fw-semibold)',
      color: 'var(--text-primary)',
      lineHeight: 1.2
    }
  }, v, unit), unit === '%' && /*#__PURE__*/React.createElement("div", {
    style: {
      height: 2,
      background: 'var(--surface-3)',
      borderRadius: 1,
      marginTop: 4
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: '100%',
      width: `${v}%`,
      background: v > 85 ? 'var(--danger)' : 'var(--accent)',
      borderRadius: 1
    }
  })))))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      padding: '8px 10px',
      background: 'var(--surface-2)',
      borderRadius: 'var(--radius-sm)'
    }
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: "gamepad",
    size: 16,
    color: discordTracking ? 'var(--accent)' : 'var(--text-tertiary)'
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'block',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--text-tertiary)'
    }
  }, "Discord Rich Presence"), /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'block',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      fontWeight: 'var(--fw-semibold)',
      color: 'var(--text-primary)',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap'
    }
  }, discordTracking ? discordApp || 'Nada rodando' : 'Desativado')), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: e => {
      e.stopPropagation();
      onToggleTracking && onToggleTracking();
    },
    style: {
      width: 34,
      height: 20,
      borderRadius: 'var(--radius-pill)',
      border: 'none',
      cursor: 'pointer',
      padding: 2,
      backgroundColor: discordTracking ? 'var(--accent)' : 'var(--surface-3)',
      display: 'flex',
      justifyContent: discordTracking ? 'flex-end' : 'flex-start',
      transition: 'background-color var(--dur-fast)'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      width: 16,
      height: 16,
      borderRadius: '50%',
      background: '#fff'
    }
  }))), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: e => {
      e.stopPropagation();
      onOpenLauncher && onOpenLauncher();
    },
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 7,
      height: 34,
      background: 'var(--accent)',
      color: 'var(--text-on-accent)',
      border: 'none',
      borderRadius: 'var(--radius-md)',
      cursor: 'pointer',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      fontWeight: 'var(--fw-semibold)'
    }
  }, "Abrir tudo", /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 'var(--fs-caption)',
      opacity: .8
    }
  }, hotkeyLabel))));
}

/**
 * One taskbar slot: a square hit target holding a glyph, with the Windows 11
 * taskbar's hover wash, its pressed wash, and its accent underline indicator.
 *   running → underline present (16px)
 *   active  → underline widens to 20px, the way the focused app's does
 * Passing `label` expands the slot horizontally (peek state).
 * Sizes come from the --taskbar-* tokens; nothing here is a literal.
 */
function Slot({
  icon,
  color,
  label,
  labelColor,
  running,
  active,
  badge,
  indicatorColor
}) {
  const [hover, setHover] = React.useState(false);
  const [press, setPress] = React.useState(false);
  return /*#__PURE__*/React.createElement("span", {
    onMouseEnter: () => setHover(true),
    onMouseLeave: () => {
      setHover(false);
      setPress(false);
    },
    onMouseDown: () => setPress(true),
    onMouseUp: () => setPress(false),
    style: {
      position: 'relative',
      display: 'inline-flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 8,
      height: 'var(--taskbar-slot)',
      minWidth: 'var(--taskbar-slot)',
      padding: label ? '0 10px' : 0,
      borderRadius: 'var(--taskbar-slot-radius)',
      backgroundColor: press ? 'var(--taskbar-active)' : hover ? 'var(--taskbar-hover)' : 'transparent',
      transition: 'background-color var(--dur-fast) var(--ease-out), min-width var(--dur-med) var(--ease-spring)'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'relative',
      display: 'inline-flex'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      width: 'var(--taskbar-icon)',
      height: 'var(--taskbar-icon)'
    }
  }, /*#__PURE__*/React.createElement(__ds_scope.Icon, {
    name: icon,
    size: "100%",
    color: color
  })), badge != null && /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      top: -3,
      right: -6,
      minWidth: 15,
      height: 15,
      padding: '0 4px',
      borderRadius: 'var(--radius-pill)',
      background: 'var(--accent)',
      color: 'var(--text-on-accent)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      fontFamily: 'var(--font-mono)',
      fontSize: 9.5,
      fontWeight: 'var(--fw-bold)',
      lineHeight: 1
    }
  }, badge)), label && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-medium)',
      color: labelColor || 'var(--text-secondary)',
      whiteSpace: 'nowrap'
    }
  }, label), running && /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      bottom: 2,
      left: '50%',
      transform: 'translateX(-50%)',
      width: active ? 'var(--taskbar-indicator-w-active)' : 'var(--taskbar-indicator-w)',
      height: 'var(--taskbar-indicator-h)',
      borderRadius: 2,
      background: indicatorColor || 'var(--accent)',
      transition: 'width var(--dur-med) var(--ease-spring)'
    }
  }));
}
function capStyle(danger) {
  return {
    fontFamily: 'var(--font-body)',
    fontSize: 'var(--fs-caption)',
    fontWeight: 'var(--fw-medium)',
    color: danger ? 'var(--danger)' : 'var(--text-secondary)',
    whiteSpace: 'nowrap'
  };
}
function rowBtn(danger) {
  return {
    display: 'flex',
    alignItems: 'center',
    gap: 9,
    width: '100%',
    height: 36,
    padding: '0 10px',
    background: 'var(--surface-2)',
    border: '1px solid var(--border-subtle)',
    borderRadius: 'var(--radius-sm)',
    cursor: 'pointer',
    color: danger ? 'var(--danger)' : 'var(--text-primary)',
    fontFamily: 'var(--font-body)',
    fontSize: 'var(--fs-body)',
    fontWeight: 'var(--fw-medium)'
  };
}
function labelStyle() {
  return {
    fontFamily: 'var(--font-body)',
    fontSize: 'var(--fs-caption)',
    fontWeight: 'var(--fw-semibold)',
    color: 'var(--text-tertiary)',
    textTransform: 'uppercase',
    letterSpacing: 'var(--tracking-caps)',
    marginBottom: 6
  };
}
Object.assign(__ds_scope, { Notch });
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/surface/Notch.jsx", error: String((e && e.message) || e) }); }

// components/surface/notch-spec.jsx
try { (() => {
/* Shared harness for the notch spec cards: a desk-like backdrop so the acrylic
   has something to blur, plus annotation primitives (dimension rules and
   callout labels). Loaded by each notch-*.card.html. */
const NotchSpec = (() => {
  const DS = window.CortinhosDesignSystem_c0acc5;
  const SAMPLE = {
    quickShortcuts: [{
      name: 'Discord',
      type: 'app'
    }, {
      name: 'Steam',
      type: 'app'
    }, {
      name: 'Screenshots',
      type: 'folder'
    }, {
      name: 'GitHub',
      type: 'url'
    }],
    notifications: [{
      title: 'Discord',
      body: 'Lucky mencionou você em #geral',
      time: 'agora'
    }, {
      title: 'Steam',
      body: 'Counter-Strike 2 foi atualizado',
      time: '4m'
    }],
    perf: {
      cpu: 34,
      gpu: 78,
      fps: 144
    }
  };

  /* A stripe-textured desk. The stripes are the tell: wherever acrylic sits,
     they blur. */
  function Desk({
    children,
    height = 250,
    width = 260
  }) {
    return /*#__PURE__*/React.createElement("div", {
      style: {
        position: 'relative',
        width,
        height,
        flexShrink: 0,
        overflow: 'hidden',
        borderRadius: '0 0 12px 12px',
        border: '1px solid var(--border-subtle)',
        borderTop: 'none',
        background: 'linear-gradient(135deg,#1d3a52,#2b1f42 45%,#0f4a3f 78%,#432617)'
      }
    }, /*#__PURE__*/React.createElement("div", {
      style: {
        position: 'absolute',
        inset: 0,
        background: 'repeating-linear-gradient(58deg,rgba(255,255,255,.10) 0 3px,transparent 3px 24px)'
      }
    }), /*#__PURE__*/React.createElement("div", {
      style: {
        position: 'absolute',
        top: 0,
        left: 0,
        right: 0,
        display: 'flex',
        justifyContent: 'center'
      }
    }, children));
  }

  /* Horizontal dimension rule with a centred value. */
  function DimX({
    value,
    width,
    top = 44,
    left = '50%'
  }) {
    return /*#__PURE__*/React.createElement("div", {
      style: {
        position: 'absolute',
        top,
        left,
        transform: left === '50%' ? 'translateX(-50%)' : 'none',
        width,
        height: 0,
        borderTop: '1px dashed var(--accent)'
      }
    }, ['-3px', 'calc(100% - 3px)'].map((x, i) => /*#__PURE__*/React.createElement("span", {
      key: i,
      style: {
        position: 'absolute',
        left: x,
        top: -3,
        width: 6,
        height: 6,
        borderRadius: '50%',
        background: 'var(--accent)'
      }
    })), /*#__PURE__*/React.createElement("span", {
      style: {
        position: 'absolute',
        left: '50%',
        top: 5,
        transform: 'translateX(-50%)',
        fontFamily: 'var(--font-mono)',
        fontSize: 9.5,
        color: 'var(--accent)',
        whiteSpace: 'nowrap',
        background: 'var(--bg-base)',
        padding: '1px 4px',
        borderRadius: 3
      }
    }, value));
  }

  /* Callout: a leader line from the pill out to a label. */
  function Note({
    text,
    top,
    left,
    align = 'left'
  }) {
    return /*#__PURE__*/React.createElement("div", {
      style: {
        position: 'absolute',
        top,
        left,
        right: align === 'right' ? 8 : undefined,
        maxWidth: 150,
        textAlign: align,
        fontFamily: 'var(--font-body)',
        fontSize: 10,
        lineHeight: 1.4,
        color: 'var(--text-secondary)'
      }
    }, text);
  }

  /* Right-hand spec sheet: state name, what triggers it, and the tokens used. */
  function Sheet({
    title,
    trigger,
    rows
  }) {
    return /*#__PURE__*/React.createElement("div", {
      style: {
        flex: 1,
        minWidth: 0,
        display: 'flex',
        flexDirection: 'column',
        gap: 8,
        paddingTop: 2
      }
    }, /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: 'var(--font-display)',
        fontSize: 16,
        fontWeight: 'var(--fw-semibold)',
        color: 'var(--text-primary)'
      }
    }, title), /*#__PURE__*/React.createElement("div", {
      style: {
        fontFamily: 'var(--font-body)',
        fontSize: 11,
        color: 'var(--text-secondary)',
        marginTop: 2,
        lineHeight: 1.45
      }
    }, trigger)), /*#__PURE__*/React.createElement("table", {
      style: {
        borderCollapse: 'collapse',
        width: '100%'
      }
    }, /*#__PURE__*/React.createElement("tbody", null, rows.map(([k, v], i) => /*#__PURE__*/React.createElement("tr", {
      key: i
    }, /*#__PURE__*/React.createElement("td", {
      style: {
        padding: '3px 10px 3px 0',
        verticalAlign: 'top',
        whiteSpace: 'nowrap',
        fontFamily: 'var(--font-body)',
        fontSize: 10,
        fontWeight: 'var(--fw-semibold)',
        color: 'var(--text-tertiary)',
        textTransform: 'uppercase',
        letterSpacing: '.06em'
      }
    }, k), /*#__PURE__*/React.createElement("td", {
      style: {
        padding: '3px 0',
        fontFamily: 'var(--font-mono)',
        fontSize: 10,
        color: 'var(--text-primary)',
        lineHeight: 1.5,
        wordBreak: 'break-word'
      }
    }, v))))));
  }
  function Layout({
    children
  }) {
    return /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        gap: 20,
        alignItems: 'flex-start',
        padding: '0 20px 20px'
      }
    }, children);
  }
  return {
    ...DS,
    SAMPLE,
    Desk,
    DimX,
    Note,
    Sheet,
    Layout
  };
})();
})(); } catch (e) { __ds_ns.__errors.push({ path: "components/surface/notch-spec.jsx", error: String((e && e.message) || e) }); }

// ui_kits/launcher/LauncherApp.jsx
try { (() => {
/* Cortinhos Launcher — interactive UI-kit recreation.
   Composes the design-system primitives from the bundle namespace.
   Exported to window for the index.html host. */
const DS = window.CortinhosDesignSystem_c0acc5;
const {
  ShortcutTile,
  Badge,
  Icon,
  Button,
  TextField,
  Select,
  Checkbox,
  ThemeToggle,
  Dialog,
  ContextMenu,
  Tooltip,
  Notch
} = DS;
const TYPE_OPTS = [{
  value: 'app',
  label: 'Aplicativo',
  iconName: 'app'
}, {
  value: 'folder',
  label: 'Pasta',
  iconName: 'folder'
}, {
  value: 'url',
  label: 'URL',
  iconName: 'url'
}, {
  value: 'command',
  label: 'Comando',
  iconName: 'command'
}];
const SEED = [{
  id: 1,
  name: 'Discord',
  type: 'app',
  hotkey: 'Ctrl+Alt+1',
  running: true,
  track: true
}, {
  id: 2,
  name: 'Steam',
  type: 'app',
  running: false
}, {
  id: 3,
  name: 'Counter-Strike 2',
  type: 'app',
  hotkey: 'Ctrl+Alt+2',
  running: true,
  track: true
}, {
  id: 4,
  name: 'Screenshots',
  type: 'folder',
  pinned: true
}, {
  id: 5,
  name: 'OBS Studio',
  type: 'app'
}, {
  id: 6,
  name: 'GitHub Ambiensys',
  type: 'url'
}, {
  id: 7,
  name: 'Spotify',
  type: 'app'
}, {
  id: 8,
  name: 'Limpar temp',
  type: 'command'
}];
function WindowBar({
  tab,
  setTab,
  count,
  theme,
  onTheme,
  query,
  setQuery
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      background: 'var(--material-mica)',
      borderBottom: '1px solid var(--border-subtle)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 16,
      padding: '0 0 0 20px',
      minHeight: 44
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'flex',
      width: 24,
      height: 24
    },
    dangerouslySetInnerHTML: {
      __html: '<svg viewBox="0 0 128 128" width="24" height="24" aria-label="Cortinhos"><defs><pattern id="lb-p" width="28.16" height="128" patternUnits="userSpaceOnUse"><rect width="14.08" height="128" fill="#3ea6ff"/><rect x="14.08" width="14.08" height="128" fill="#2f8fe0"/></pattern><clipPath id="lb-c"><rect width="128" height="128" rx="28"/></clipPath></defs><g clip-path="url(#lb-c)"><rect width="128" height="128" fill="url(#lb-p)"/><path d="M30.72 0 H97.28 V14.08 a21.76 21.76 0 0 1 -21.76 21.76 H52.48 a21.76 21.76 0 0 1 -21.76 -21.76 Z" fill="#14171c"/><circle cx="53" cy="17.5" r="5.6" fill="#5cb4ff"/><circle cx="75" cy="17.5" r="5.6" fill="#fff" opacity="0.6"/><path d="M52.75 49.5L77.25 49.5L85.75 58L85.5 65.75L76.25 65.75L76 61.75L72.25 58L57.25 58.25L53.75 61.75L53.75 89.25L57.75 93L72.25 93L76 89.25L76 85.25L85.25 85L85.75 85.25L85.75 93L77.5 101.25L52.25 101.25L44 93L44 58L52 49.75L52.5 49.75Z" fill="#fff"/></g></svg>'
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-display)',
      fontWeight: 700,
      fontSize: 19,
      color: 'var(--text-primary)',
      letterSpacing: '.02em'
    }
  }, "cor", /*#__PURE__*/React.createElement("span", {
    style: {
      color: 'var(--accent)'
    }
  }, "tinhos"))), /*#__PURE__*/React.createElement(Badge, {
    mono: true,
    tone: "accent"
  }, "Ctrl+Alt+L"), /*#__PURE__*/React.createElement(Badge, {
    tone: "neutral"
  }, count, " atalhos"), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }), /*#__PURE__*/React.createElement(ThemeToggle, {
    theme: theme,
    onChange: onTheme
  }), /*#__PURE__*/React.createElement(CaptionButtons, null)), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 4,
      padding: '0 20px 12px'
    }
  }, [['grid', 'Atalhos'], ['config', 'Config']].map(([k, l]) => /*#__PURE__*/React.createElement(Tab, {
    key: k,
    label: l,
    selected: tab === k,
    onClick: () => setTab(k)
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }), tab === 'grid' && /*#__PURE__*/React.createElement(TextField, {
    icon: /*#__PURE__*/React.createElement(Icon, {
      name: "search",
      size: 15
    }),
    placeholder: "Buscar atalho\u2026",
    value: query,
    onChange: e => setQuery(e.target.value),
    style: {
      width: 240
    }
  })));
}

/* Windows 11 caption buttons. 46×32 rectangles with NO radius — they bleed into
   the window corner. Only Close takes a colour on hover, and it is the fixed
   system red, not our --danger. Glyphs are geometric primitives (a line, a
   square outline, an X), drawn inline. */
function CaptionButton({
  kind,
  onClick
}) {
  const [hover, setHover] = React.useState(false);
  const [press, setPress] = React.useState(false);
  const isClose = kind === 'close';
  const bg = press ? isClose ? 'var(--caption-close-press)' : 'var(--caption-press)' : hover ? isClose ? 'var(--caption-close-hover)' : 'var(--caption-hover)' : 'transparent';
  const fg = isClose && (hover || press) ? '#fff' : 'var(--text-primary)';
  return /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: onClick,
    "aria-label": kind,
    onMouseEnter: () => setHover(true),
    onMouseLeave: () => {
      setHover(false);
      setPress(false);
    },
    onMouseDown: () => setPress(true),
    onMouseUp: () => setPress(false),
    style: {
      width: 'var(--caption-btn-w)',
      height: 'var(--caption-btn-h)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      border: 'none',
      borderRadius: 0,
      backgroundColor: bg,
      color: fg,
      cursor: 'default',
      transition: 'background-color var(--dur-fast) var(--ease-out)'
    }
  }, /*#__PURE__*/React.createElement("svg", {
    width: "10",
    height: "10",
    viewBox: "0 0 10 10",
    fill: "none",
    stroke: "currentColor",
    strokeWidth: "1"
  }, kind === 'minimize' && /*#__PURE__*/React.createElement("path", {
    d: "M0 5h10"
  }), kind === 'maximize' && /*#__PURE__*/React.createElement("rect", {
    x: "0.5",
    y: "0.5",
    width: "9",
    height: "9"
  }), kind === 'close' && /*#__PURE__*/React.createElement("path", {
    d: "M0 0l10 10M10 0L0 10"
  })));
}
function CaptionButtons() {
  return /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'flex',
      alignSelf: 'flex-start'
    }
  }, /*#__PURE__*/React.createElement(CaptionButton, {
    kind: "minimize"
  }), /*#__PURE__*/React.createElement(CaptionButton, {
    kind: "maximize"
  }), /*#__PURE__*/React.createElement(CaptionButton, {
    kind: "close"
  }));
}

/* Selection uses the SAME accent-underline grammar as the notch's taskbar
   slots, so one product speaks one selection language. */
function Tab({
  label,
  selected,
  onClick
}) {
  const [hover, setHover] = React.useState(false);
  return /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: onClick,
    onMouseEnter: () => setHover(true),
    onMouseLeave: () => setHover(false),
    style: {
      position: 'relative',
      padding: '7px 12px 9px',
      border: 'none',
      cursor: 'pointer',
      borderRadius: 'var(--radius-sm)',
      backgroundColor: hover && !selected ? 'var(--taskbar-hover)' : 'transparent',
      fontFamily: 'var(--font-body)',
      fontSize: 13,
      fontWeight: 600,
      color: selected ? 'var(--text-primary)' : 'var(--text-secondary)',
      transition: 'background-color var(--dur-fast) var(--ease-out), color var(--dur-fast)'
    }
  }, label, /*#__PURE__*/React.createElement("span", {
    style: {
      position: 'absolute',
      bottom: 2,
      left: '50%',
      transform: 'translateX(-50%)',
      width: selected ? 'calc(100% - 24px)' : 0,
      height: 'var(--taskbar-indicator-h)',
      borderRadius: 2,
      background: 'var(--accent)',
      transition: 'width var(--dur-med) var(--ease-spring)'
    }
  }));
}
function SettingsView() {
  const [enabled, setEnabled] = React.useState(true);
  const [client, setClient] = React.useState('123456789012345678');
  return /*#__PURE__*/React.createElement("div", {
    style: {
      padding: 24,
      display: 'flex',
      flexDirection: 'column',
      gap: 20,
      maxWidth: 520
    }
  }, /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("h3", {
    style: sectionTitle
  }, "Discord Rich Presence"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 12
    }
  }, /*#__PURE__*/React.createElement(Checkbox, {
    checked: enabled,
    onChange: setEnabled,
    label: "Ativar Rich Presence quando um jogo rastreado estiver rodando"
  }), /*#__PURE__*/React.createElement(TextField, {
    label: "Client ID",
    value: client,
    onChange: e => setClient(e.target.value),
    disabled: !enabled
  }))), /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("h3", {
    style: sectionTitle
  }, "Hotkey global"), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontSize: 13,
      color: 'var(--text-secondary)'
    }
  }, "Abrir/fechar o launcher"), /*#__PURE__*/React.createElement(Badge, {
    mono: true,
    tone: "accent"
  }, "Ctrl+Alt+L"), /*#__PURE__*/React.createElement(Button, {
    size: "sm",
    variant: "secondary"
  }, "Regravar"))), /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("h3", {
    style: sectionTitle
  }, "Bandeja"), /*#__PURE__*/React.createElement("span", {
    style: {
      fontSize: 13,
      color: 'var(--text-secondary)',
      lineHeight: 1.5
    }
  }, "Fechar a janela (X) s\xF3 minimiza pra bandeja. Use ", /*#__PURE__*/React.createElement("b", {
    style: {
      color: 'var(--text-primary)'
    }
  }, "Sair"), " no menu da bandeja pra encerrar de verdade.")));
}
const sectionTitle = {
  margin: '0 0 12px',
  fontFamily: 'var(--font-display)',
  fontWeight: 600,
  fontSize: 15,
  color: 'var(--text-primary)'
};
function TrayMenu({
  onClose,
  onOpen
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      bottom: 46,
      right: 12,
      width: 180,
      padding: 4,
      zIndex: 50,
      background: 'var(--surface-3)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-md)',
      boxShadow: 'var(--elev-4)'
    }
  }, /*#__PURE__*/React.createElement("button", {
    onClick: () => {
      onOpen();
      onClose();
    },
    style: trayItem
  }, "Abrir launcher"), /*#__PURE__*/React.createElement("div", {
    style: {
      height: 1,
      background: 'var(--border-subtle)',
      margin: '4px 0'
    }
  }), /*#__PURE__*/React.createElement("button", {
    onClick: onClose,
    style: {
      ...trayItem,
      color: 'var(--danger)'
    }
  }, "Sair"));
}
const trayItem = {
  display: 'block',
  width: '100%',
  textAlign: 'left',
  padding: '7px 10px',
  border: 'none',
  background: 'transparent',
  cursor: 'pointer',
  borderRadius: 'var(--radius-sm)',
  fontFamily: 'var(--font-body)',
  fontSize: 13,
  color: 'var(--text-primary)'
};
function LauncherApp() {
  const [theme, setTheme] = React.useState('dark');
  const [tab, setTab] = React.useState('grid');
  const [query, setQuery] = React.useState('');
  const [items, setItems] = React.useState(SEED);
  const [menu, setMenu] = React.useState(null); // {x,y,item}
  const [dialog, setDialog] = React.useState(null); // {mode,item}
  const [launching, setLaunching] = React.useState(null);
  const [tray, setTray] = React.useState(false);
  const [muted, setMuted] = React.useState(false);
  const [tracking, setTracking] = React.useState(true);
  const rootRef = React.useRef(null);
  const setThemeFor = t => {
    setTheme(t);
    if (rootRef.current) rootRef.current.dataset.theme = t;
  };
  const filtered = items.filter(i => i.name.toLowerCase().includes(query.toLowerCase()));
  const runningApp = items.find(i => i.running && i.track);
  function launch(item) {
    setLaunching(item.name);
    setTimeout(() => setLaunching(null), 1400);
  }
  function saveDialog(data) {
    if (dialog.mode === 'edit') setItems(items.map(i => i.id === dialog.item.id ? {
      ...i,
      ...data
    } : i));else setItems([...items, {
      ...data,
      id: Date.now()
    }]);
    setDialog(null);
  }
  function remove(item) {
    setItems(items.filter(i => i.id !== item.id));
  }
  return /*#__PURE__*/React.createElement("div", {
    ref: rootRef,
    "data-theme": theme,
    style: {
      position: 'relative',
      width: 'min(920px, 100%)',
      margin: '0 auto'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      top: -1,
      left: '50%',
      transform: 'translateX(-50%)',
      zIndex: 200
    }
  }, /*#__PURE__*/React.createElement(Notch, {
    micMuted: muted,
    onToggleMic: () => setMuted(m => !m),
    discordTracking: tracking,
    discordApp: runningApp ? runningApp.name : null,
    onToggleTracking: () => setTracking(t => !t),
    quickShortcuts: items.filter(i => i.pinned || i.hotkey).slice(0, 5).map(i => ({
      name: i.name,
      type: i.type,
      onOpen: () => launch(i)
    })),
    onOpenLauncher: () => {},
    hotkeyLabel: "Ctrl+Alt+L"
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      background: 'var(--bg-base)',
      borderRadius: 'var(--radius-lg)',
      overflow: 'hidden',
      border: '1px solid var(--border-subtle)',
      boxShadow: 'var(--elev-4)',
      minHeight: 600,
      display: 'flex',
      flexDirection: 'column'
    }
  }, /*#__PURE__*/React.createElement(WindowBar, {
    tab: tab,
    setTab: setTab,
    count: items.length,
    theme: theme,
    onTheme: setThemeFor,
    query: query,
    setQuery: setQuery
  }), tab === 'config' ? /*#__PURE__*/React.createElement(SettingsView, null) : /*#__PURE__*/React.createElement("div", {
    style: {
      padding: 'var(--window-pad)',
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: 'repeat(auto-fill, minmax(150px, 1fr))',
      gap: 'var(--grid-gap)'
    }
  }, filtered.map(item => /*#__PURE__*/React.createElement(ShortcutTile, {
    key: item.id,
    name: item.name,
    type: item.type,
    hotkey: item.hotkey,
    running: item.running,
    pinned: item.pinned,
    onOpen: () => launch(item),
    onContextMenu: e => {
      e.preventDefault();
      setMenu({
        x: e.clientX,
        y: e.clientY,
        item
      });
    }
  })), /*#__PURE__*/React.createElement("button", {
    onClick: () => setDialog({
      mode: 'add'
    }),
    style: {
      aspectRatio: '3 / 4',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 10,
      background: 'transparent',
      border: '1px dashed var(--border-strong)',
      borderRadius: 'var(--radius-md)',
      color: 'var(--text-tertiary)',
      cursor: 'pointer',
      fontFamily: 'var(--font-body)',
      fontSize: 13,
      fontWeight: 600
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "plus",
    size: 26
  }), " Adicionar"))), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      padding: '10px 20px',
      borderTop: '1px solid var(--border-subtle)',
      background: 'var(--bg-window)'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontSize: 11,
      color: 'var(--text-tertiary)',
      fontFamily: 'var(--font-mono)'
    }
  }, launching ? `Abrindo ${launching}…` : 'Pronto'), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }), /*#__PURE__*/React.createElement(Tooltip, {
    label: "\xCDcone da bandeja",
    placement: "top"
  }, /*#__PURE__*/React.createElement("button", {
    onClick: () => setTray(t => !t),
    style: {
      display: 'flex',
      width: 28,
      height: 28,
      alignItems: 'center',
      justifyContent: 'center',
      border: '1px solid var(--border-subtle)',
      background: 'var(--surface-1)',
      borderRadius: 'var(--radius-sm)',
      color: 'var(--text-secondary)',
      cursor: 'pointer'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "gamepad",
    size: 16
  })))), tray && /*#__PURE__*/React.createElement(TrayMenu, {
    onClose: () => setTray(false),
    onOpen: () => setTab('grid')
  })), launching && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      bottom: 60,
      left: '50%',
      transform: 'translateX(-50%)',
      zIndex: 300,
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      padding: '10px 16px',
      background: 'var(--surface-3)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-pill)',
      boxShadow: 'var(--elev-4)',
      color: 'var(--text-primary)',
      fontFamily: 'var(--font-body)',
      fontSize: 13
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "play",
    size: 15,
    color: "var(--accent)"
  }), " Abrindo ", /*#__PURE__*/React.createElement("b", null, launching)), menu && /*#__PURE__*/React.createElement(ContextMenu, {
    x: menu.x,
    y: menu.y,
    onClose: () => setMenu(null),
    items: [{
      label: 'Abrir',
      icon: /*#__PURE__*/React.createElement(Icon, {
        name: "play",
        size: 15
      }),
      onClick: () => launch(menu.item)
    }, {
      label: 'Editar',
      icon: /*#__PURE__*/React.createElement(Icon, {
        name: "edit",
        size: 15
      }),
      onClick: () => setDialog({
        mode: 'edit',
        item: menu.item
      })
    }, {
      divider: true
    }, {
      label: 'Remover',
      icon: /*#__PURE__*/React.createElement(Icon, {
        name: "trash",
        size: 15
      }),
      danger: true,
      onClick: () => remove(menu.item)
    }]
  }), dialog && /*#__PURE__*/React.createElement(ShortcutDialog, {
    dialog: dialog,
    onClose: () => setDialog(null),
    onSave: saveDialog
  }));
}
function ShortcutDialog({
  dialog,
  onClose,
  onSave
}) {
  const src = dialog.item || {};
  const [name, setName] = React.useState(src.name || '');
  const [type, setType] = React.useState(src.type || 'app');
  const [path, setPath] = React.useState('');
  const [track, setTrack] = React.useState(!!src.track);
  const [err, setErr] = React.useState('');
  function save() {
    if (!name.trim()) {
      setErr('Preencha o nome.');
      return;
    }
    onSave({
      name: name.trim(),
      type,
      track: type === 'app' && track
    });
  }
  return /*#__PURE__*/React.createElement(Dialog, {
    open: true,
    title: dialog.mode === 'edit' ? 'Editar atalho' : 'Adicionar atalho',
    onClose: onClose,
    footer: /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement(Button, {
      variant: "ghost",
      onClick: onClose
    }, "Cancelar"), /*#__PURE__*/React.createElement(Button, {
      variant: "primary",
      onClick: save
    }, "Salvar"))
  }, /*#__PURE__*/React.createElement(TextField, {
    label: "Nome",
    placeholder: "Ex: Discord",
    value: name,
    error: err,
    onChange: e => {
      setName(e.target.value);
      setErr('');
    }
  }), /*#__PURE__*/React.createElement(Select, {
    label: "Tipo",
    value: type,
    onChange: setType,
    options: TYPE_OPTS.map(o => ({
      value: o.value,
      label: o.label,
      icon: /*#__PURE__*/React.createElement(Icon, {
        name: o.iconName,
        size: 16
      })
    }))
  }), /*#__PURE__*/React.createElement(TextField, {
    label: "Caminho",
    placeholder: "C:\\\\\u2026 ou https://\u2026",
    value: path,
    icon: /*#__PURE__*/React.createElement(Icon, {
      name: type,
      size: 16
    }),
    onChange: e => setPath(e.target.value),
    trailing: type !== 'url' && type !== 'command' ? /*#__PURE__*/React.createElement(Button, {
      size: "sm",
      variant: "secondary"
    }, "Procurar") : null
  }), type === 'app' && /*#__PURE__*/React.createElement(Checkbox, {
    checked: track,
    onChange: setTrack,
    label: "Rastrear presen\xE7a no Discord"
  }));
}
window.LauncherApp = LauncherApp;
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/launcher/LauncherApp.jsx", error: String((e && e.message) || e) }); }

// ui_kits/overlay/Feasibility.jsx
try { (() => {
/* Nota de viabilidade técnica — overlay em tela cheia no WPF/.NET 8.
   Conteúdo de decisão, não UI de produto: por isso o visual é de documento. */
const FIcon = window.CortinhosDesignSystem_c0acc5.Icon;
function FeasibilityPanel({
  onClose
}) {
  return /*#__PURE__*/React.createElement("div", {
    onClick: onClose,
    style: {
      position: 'fixed',
      inset: 0,
      zIndex: 200,
      background: 'rgba(6,8,11,.7)',
      backdropFilter: 'blur(3px)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      padding: 32
    }
  }, /*#__PURE__*/React.createElement("div", {
    onClick: e => e.stopPropagation(),
    style: {
      width: 'min(780px, 100%)',
      maxHeight: '100%',
      overflow: 'auto',
      background: 'var(--bg-window)',
      border: '1px solid var(--border-strong)',
      borderRadius: 'var(--radius-lg)',
      boxShadow: 'var(--elev-4)',
      padding: '28px 32px 32px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'flex-start',
      gap: 16,
      marginBottom: 20
    }
  }, /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 600,
      textTransform: 'uppercase',
      letterSpacing: 'var(--tracking-caps)',
      color: 'var(--text-tertiary)'
    }
  }, "Nota t\xE9cnica"), /*#__PURE__*/React.createElement("h2", {
    style: {
      margin: '4px 0 0',
      fontFamily: 'var(--font-display)',
      fontWeight: 700,
      fontSize: 26,
      color: 'var(--text-primary)'
    }
  }, "Overlay em tela cheia \u2014 d\xE1 pra fazer?")), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: onClose,
    style: {
      background: 'none',
      border: 'none',
      cursor: 'pointer',
      color: 'var(--text-secondary)',
      padding: 4
    }
  }, /*#__PURE__*/React.createElement(FIcon, {
    name: "x",
    size: 22
  }))), /*#__PURE__*/React.createElement(Verdict, null), /*#__PURE__*/React.createElement(Sec, {
    n: "1",
    t: "A janela do overlay"
  }, /*#__PURE__*/React.createElement("p", null, "Uma janela sem chrome do tamanho do monitor: ", /*#__PURE__*/React.createElement(M, null, "WindowStyle=None"), ", ", /*#__PURE__*/React.createElement(M, null, "AllowsTransparency=True"), ", ", /*#__PURE__*/React.createElement(M, null, "Topmost=True"), ", ", /*#__PURE__*/React.createElement(M, null, "ShowInTaskbar=False"), ". Funciona, mas ", /*#__PURE__*/React.createElement("b", null, "AllowsTransparency for\xE7a renderiza\xE7\xE3o por software"), " em WPF \u2014 em tela cheia isso custa caro e some com a acelera\xE7\xE3o."), /*#__PURE__*/React.createElement("p", null, "Caminho recomendado: janela Win32 ", /*#__PURE__*/React.createElement(M, null, "WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE"), " com composi\xE7\xE3o por ", /*#__PURE__*/React.createElement(M, null, "DirectComposition"), " / ", /*#__PURE__*/React.createElement(M, null, "HwndHost"), ", mantendo o conte\xFAdo WPF acelerado. Mais trabalho de plumbing, mas \xE9 o que sustenta 60fps sobre um jogo.")), /*#__PURE__*/React.createElement(Sec, {
    n: "2",
    t: "Clic\xE1vel s\xF3 nas ilhas",
    tone: "watch"
  }, /*#__PURE__*/React.createElement("p", null, /*#__PURE__*/React.createElement(M, null, "WS_EX_TRANSPARENT"), " \xE9 tudo-ou-nada: liga ou desliga o hit-test da ", /*#__PURE__*/React.createElement("i", null, "janela inteira"), ". Duas rotas pra \u201Cs\xF3 as ilhas capturam clique\u201D:"), /*#__PURE__*/React.createElement("ul", null, /*#__PURE__*/React.createElement("li", null, /*#__PURE__*/React.createElement("b", null, "Hook de mouse global"), " (", /*#__PURE__*/React.createElement(M, null, "SetWindowsHookEx(WH_MOUSE_LL)"), "): a cada movimento, testa se o cursor est\xE1 dentro de alguma ilha e liga/desliga ", /*#__PURE__*/React.createElement(M, null, "WS_EX_TRANSPARENT"), ". Barato, \xE9 o que a maioria dos overlays faz. Cuidado: hook de baixo n\xEDvel roda na thread de UI \u2014 qualquer travada sua vira lag de mouse no sistema inteiro."), /*#__PURE__*/React.createElement("li", null, /*#__PURE__*/React.createElement("b", null, "Regi\xE3o da janela"), " (", /*#__PURE__*/React.createElement(M, null, "SetWindowRgn"), " com a uni\xE3o dos ret\xE2ngulos das ilhas): preciso e sem hook, mas recalcular a cada frame de drag \xE9 pesado e a regi\xE3o \xE9 hard-edged \u2014 mata sombra, blur e canto arredondado nas bordas.")), /*#__PURE__*/React.createElement("p", null, /*#__PURE__*/React.createElement("b", null, "Recomendo o hook"), ", com o bounding box das ilhas em cache e recalculado s\xF3 quando o layout muda.")), /*#__PURE__*/React.createElement(Sec, {
    n: "3",
    t: "O escurecimento \xE9 o n\xF3 da ideia",
    tone: "watch"
  }, /*#__PURE__*/React.createElement("p", null, "Um scrim de tela cheia \xE9 pixel pintado: a janela precisa cobrir a tela toda. Isso n\xE3o impede o click-through (o hook decide por posi\xE7\xE3o, n\xE3o por pixel), mas significa que ", /*#__PURE__*/React.createElement("b", null, "o overlay sempre desenha a tela inteira"), " \u2014 n\xE3o d\xE1 pra \u201Cs\xF3 desenhar as ilhas\u201D quando h\xE1 scrim."), /*#__PURE__*/React.createElement("p", null, "Consequ\xEAncias pr\xE1ticas: composi\xE7\xE3o de tela cheia a cada frame sobre o jogo, e ", /*#__PURE__*/React.createElement(M, null, "backdrop blur"), " global invi\xE1vel (o DWM n\xE3o exp\xF5e blur barato do que est\xE1 atr\xE1s em fullscreen; ", /*#__PURE__*/React.createElement(M, null, "DwmEnableBlurBehindWindow"), " est\xE1 deprecado)."), /*#__PURE__*/React.createElement("p", null, /*#__PURE__*/React.createElement("b", null, "Por isso o desenho aqui usa scrim liso + acrylic s\xF3 nas ilhas"), " \u2014 \xE1reas pequenas, custo aceit\xE1vel. Se o custo pesar, a op\xE7\xE3o ", /*#__PURE__*/React.createElement("i", null, "Halo local"), " resolve sem scrim algum.")), /*#__PURE__*/React.createElement(Sec, {
    n: "4",
    t: "Fullscreen exclusivo: n\xE3o",
    tone: "no"
  }, /*#__PURE__*/React.createElement("p", null, "Overlay por janela topmost ", /*#__PURE__*/React.createElement("b", null, "n\xE3o aparece"), " sobre DirectX em fullscreen exclusivo. Aparece normalmente em ", /*#__PURE__*/React.createElement("b", null, "borderless windowed"), " e em fullscreen com as \u201Cotimiza\xE7\xF5es para jogos em janela\u201D do Windows 10/11 \u2014 que hoje \xE9 o caso da grande maioria dos jogos."), /*#__PURE__*/React.createElement("p", null, "Cobrir exclusivo exigiria hook de swapchain (", /*#__PURE__*/React.createElement(M, null, "IDXGISwapChain::Present"), "), o mesmo territ\xF3rio de Steam/Discord overlay. ", /*#__PURE__*/React.createElement("b", null, "Anti-cheat (EAC, BattlEye) trata isso como inje\xE7\xE3o"), " \u2014 risco real de ban pro usu\xE1rio. Recomenda\xE7\xE3o: ", /*#__PURE__*/React.createElement("b", null, "n\xE3o fazer"), ", e dizer isso na tela de config.")), /*#__PURE__*/React.createElement(Sec, {
    n: "5",
    t: "Teclado e foco",
    tone: "watch"
  }, /*#__PURE__*/React.createElement("p", null, /*#__PURE__*/React.createElement(M, null, "WS_EX_NOACTIVATE"), " mant\xE9m o jogo em foco \u2014 bom pro overlay n\xE3o roubar input. Mas a\xED a ", /*#__PURE__*/React.createElement("b", null, "busca n\xE3o recebe teclas"), ". Solu\xE7\xE3o (a mesma do Discord): ao focar o campo de busca, chamar ", /*#__PURE__*/React.createElement(M, null, "SetForegroundWindow"), " no overlay e devolver o foco ao jogo quando fechar. O hotkey global (", /*#__PURE__*/React.createElement(M, null, "RegisterHotKey"), ", j\xE1 existe no app) continua funcionando dos dois lados.")), /*#__PURE__*/React.createElement(Sec, {
    n: "6",
    t: "Layout arrast\xE1vel e salvo",
    tone: "ok"
  }, /*#__PURE__*/React.createElement("p", null, "Trivial. Posi\xE7\xF5es em % do monitor escolhido, num ", /*#__PURE__*/React.createElement(M, null, "overlay-layout.json"), " ao lado do ", /*#__PURE__*/React.createElement(M, null, "shortcuts.json"), ". Guardar em % (n\xE3o px) faz o arranjo sobreviver a troca de resolu\xE7\xE3o e a monitores diferentes.")), /*#__PURE__*/React.createElement(Sec, {
    n: "7",
    t: "Perf module",
    tone: "watch"
  }, /*#__PURE__*/React.createElement("p", null, "Continua sem fonte definida. CPU/GPU via ", /*#__PURE__*/React.createElement("b", null, "PDH counters"), " ou ", /*#__PURE__*/React.createElement("b", null, "LibreHardwareMonitor"), " \xE9 seguro. ", /*#__PURE__*/React.createElement("b", null, "FPS n\xE3o"), ": medir FPS exige hook de Present \u2014 mesmo problema de anti-cheat do item 4. Sugest\xE3o: cortar FPS da ilha, ou mostrar s\xF3 quando o usu\xE1rio instalar um provedor externo.")), /*#__PURE__*/React.createElement(Sec, {
    n: "8",
    t: "O que isso muda no design system",
    tone: "watch"
  }, /*#__PURE__*/React.createElement("ul", null, /*#__PURE__*/React.createElement("li", null, /*#__PURE__*/React.createElement("b", null, "Mica sai de cena."), " Se a janela some, o material de superf\xEDcie longa-dura\xE7\xE3o perde uso \u2014 o sistema passa a ter s\xF3 acrylic. Vale registrar isso em ", /*#__PURE__*/React.createElement(M, null, "tokens/platform.css"), "."), /*#__PURE__*/React.createElement("li", null, /*#__PURE__*/React.createElement("b", null, "Chrome nativo some junto."), " Caption buttons, barra de t\xEDtulo, abas \u2014 todo o vocabul\xE1rio \u201Capp do Windows\u201D deixa de existir. O produto vira 100% OS-furniture."), /*#__PURE__*/React.createElement("li", null, /*#__PURE__*/React.createElement("b", null, "Perde a casa das configs."), " Hoje a aba Config mora na janela. No overlay ela precisa virar ilha ou flyout \u2014 a ilha de engrenagem aqui \xE9 um marcador, n\xE3o uma solu\xE7\xE3o."), /*#__PURE__*/React.createElement("li", null, /*#__PURE__*/React.createElement("b", null, "Notch e overlay se sobrep\xF5em."), " Se o notch expandido j\xE1 traz mic, notifica\xE7\xF5es, perf e Discord, o overlay em modo \u201CSeparado\u201D duplica quatro superf\xEDcies. Escolher ", /*#__PURE__*/React.createElement("i", null, "Ilha-m\xE3e"), " resolve, mas ent\xE3o o overlay \xE9 essencialmente \u201Cnotch expandido + grid\u201D."))), /*#__PURE__*/React.createElement(Sec, {
    n: "9",
    t: "Recomenda\xE7\xE3o",
    tone: "ok"
  }, /*#__PURE__*/React.createElement("p", null, /*#__PURE__*/React.createElement("b", null, "Vi\xE1vel, com dois cortes."), " Fazer: janela layered em tela cheia, click-through por hook, scrim liso, acrylic s\xF3 nas ilhas, layout arrast\xE1vel em %, borderless-only. N\xE3o fazer: hook de swapchain (fullscreen exclusivo e FPS)."), /*#__PURE__*/React.createElement("p", null, "Sobre a substitui\xE7\xE3o total da janela: o overlay resolve muito bem o caso ", /*#__PURE__*/React.createElement("i", null, "\u201Cestou no jogo, quero abrir algo r\xE1pido\u201D"), ". Ele resolve mal ", /*#__PURE__*/React.createElement("i", null, "\u201Cquero cadastrar 20 atalhos, editar hotkeys, mexer nas configs\u201D"), ". Sugiro ", /*#__PURE__*/React.createElement("b", null, "manter a janela pra gerenciar"), " e deixar o overlay ser a cara do uso di\xE1rio \u2014 mas isso \xE9 decis\xE3o sua, e o prot\xF3tipo est\xE1 pronto pros dois caminhos."))));
}
function Verdict() {
  const items = [['ok', 'Ilhas flutuantes + layout arrastável', 'Sem obstáculo.'], ['ok', 'Escurecer a tela', 'Custa composição de tela cheia, mas funciona.'], ['watch', 'Clique só nas ilhas', 'Precisa de hook de mouse global.'], ['watch', 'Blur do que está atrás', 'Só nas ilhas — global é inviável.'], ['no', 'Sobre fullscreen exclusivo', 'Exigiria injeção. Risco de anti-cheat.'], ['no', 'FPS no módulo de perf', 'Mesmo problema. CPU/GPU seguem OK.']];
  const tone = {
    ok: 'var(--success)',
    watch: 'var(--warning)',
    no: 'var(--danger)'
  };
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: '1fr 1fr',
      gap: 8,
      marginBottom: 26
    }
  }, items.map(([t, k, v]) => /*#__PURE__*/React.createElement("div", {
    key: k,
    style: {
      display: 'flex',
      gap: 10,
      padding: '10px 12px',
      background: 'var(--surface-1)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-sm)'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      width: 8,
      height: 8,
      borderRadius: '50%',
      background: tone[t],
      marginTop: 5,
      flexShrink: 0
    }
  }), /*#__PURE__*/React.createElement("span", null, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'block',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      fontWeight: 600,
      color: 'var(--text-primary)'
    }
  }, k), /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'block',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--text-secondary)',
      marginTop: 2
    }
  }, v)))));
}
function Sec({
  n,
  t,
  tone,
  children
}) {
  const c = tone === 'no' ? 'var(--danger)' : tone === 'watch' ? 'var(--warning)' : tone === 'ok' ? 'var(--success)' : 'var(--accent)';
  return /*#__PURE__*/React.createElement("section", {
    style: {
      marginBottom: 22
    }
  }, /*#__PURE__*/React.createElement("h3", {
    style: {
      display: 'flex',
      alignItems: 'baseline',
      gap: 9,
      margin: '0 0 8px',
      fontFamily: 'var(--font-display)',
      fontWeight: 600,
      fontSize: 17,
      color: 'var(--text-primary)'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 12,
      color: c
    }
  }, n), t), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      lineHeight: 1.62,
      color: 'var(--text-secondary)',
      textWrap: 'pretty'
    }
  }, children));
}
function M({
  children
}) {
  return /*#__PURE__*/React.createElement("code", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 12,
      color: 'var(--text-primary)',
      background: 'var(--surface-2)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 4,
      padding: '1px 5px'
    }
  }, children);
}
window.FeasibilityPanel = FeasibilityPanel;
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/overlay/Feasibility.jsx", error: String((e && e.message) || e) }); }

// ui_kits/overlay/Islands.jsx
try { (() => {
/* Cortinhos — Overlay islands.
   Each island is the CONTENT only; positioning, chrome and drag live in
   OverlayApp.jsx's <Island> wrapper. Composes design-system primitives. */
const ODS = window.CortinhosDesignSystem_c0acc5;
const {
  ShortcutTile,
  Badge,
  Icon,
  Tooltip
} = ODS;
const OVERLAY_SEED = [{
  id: 1,
  name: 'Discord',
  type: 'app',
  hotkey: 'Ctrl+Alt+1',
  running: true,
  pinned: true
}, {
  id: 2,
  name: 'Steam',
  type: 'app',
  running: true,
  pinned: true
}, {
  id: 3,
  name: 'Valorant',
  type: 'app',
  hotkey: 'Ctrl+Alt+2',
  running: false
}, {
  id: 4,
  name: 'OBS Studio',
  type: 'app',
  running: true,
  pinned: true
}, {
  id: 5,
  name: 'Spotify',
  type: 'app',
  running: true
}, {
  id: 6,
  name: 'Prints',
  type: 'folder',
  pinned: true
}, {
  id: 7,
  name: 'Clipes',
  type: 'folder'
}, {
  id: 8,
  name: 'Mods',
  type: 'folder'
}, {
  id: 9,
  name: 'Twitch',
  type: 'url',
  hotkey: 'Ctrl+Alt+3'
}, {
  id: 10,
  name: 'YouTube',
  type: 'url'
}, {
  id: 11,
  name: 'Painel do bot',
  type: 'url'
}, {
  id: 12,
  name: 'Reiniciar áudio',
  type: 'command',
  pinned: true
}, {
  id: 13,
  name: 'Limpar shader cache',
  type: 'command'
}, {
  id: 14,
  name: 'Modo jogo',
  type: 'command',
  hotkey: 'Ctrl+Alt+4'
}];
const islandLabel = {
  fontFamily: 'var(--font-body)',
  fontSize: 'var(--fs-caption)',
  fontWeight: 'var(--fw-semibold)',
  color: 'var(--text-tertiary)',
  textTransform: 'uppercase',
  letterSpacing: 'var(--tracking-caps)'
};
const islandCard = {
  background: 'var(--surface-2)',
  border: '1px solid var(--border-subtle)',
  borderRadius: 'var(--radius-sm)'
};

/* ── busca / command palette ─────────────────────────────────── */
function SearchIsland({
  query,
  setQuery,
  results
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      height: 44,
      padding: '0 4px'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "search",
    size: 18,
    color: "var(--text-tertiary)"
  }), /*#__PURE__*/React.createElement("input", {
    value: query,
    onChange: e => setQuery(e.target.value),
    placeholder: "Buscar atalho, pasta ou comando\u2026",
    style: {
      flex: 1,
      minWidth: 0,
      background: 'transparent',
      border: 'none',
      outline: 'none',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-subtitle)',
      color: 'var(--text-primary)'
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--text-tertiary)',
      background: 'var(--surface-2)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-sm)',
      padding: '3px 7px'
    }
  }, "Enter")), query && /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 2,
      borderTop: '1px solid var(--border-subtle)',
      paddingTop: 6,
      marginTop: 2
    }
  }, results.length === 0 && /*#__PURE__*/React.createElement("div", {
    style: {
      padding: '8px 6px',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      color: 'var(--text-tertiary)'
    }
  }, "Nada encontrado."), results.slice(0, 4).map((r, i) => /*#__PURE__*/React.createElement("div", {
    key: r.id,
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      padding: '7px 8px',
      borderRadius: 'var(--radius-sm)',
      background: i === 0 ? 'var(--accent-soft)' : 'transparent'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: r.type,
    size: 16,
    color: "var(--accent)"
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      color: 'var(--text-primary)'
    }
  }, r.name), r.hotkey && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--text-tertiary)'
    }
  }, r.hotkey)))));
}

/* ── grid de atalhos ─────────────────────────────────────────── */
function GridIsland({
  items,
  mode,
  onOpen
}) {
  if (mode === 'list') {
    return /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        flexDirection: 'column',
        height: '100%',
        minHeight: 0
      }
    }, /*#__PURE__*/React.createElement(IslandHead, {
      title: "Atalhos",
      count: items.length
    }), /*#__PURE__*/React.createElement("div", {
      style: {
        display: 'flex',
        flexDirection: 'column',
        gap: 2,
        overflow: 'auto',
        minHeight: 0
      }
    }, items.map(it => /*#__PURE__*/React.createElement(ListRow, {
      key: it.id,
      item: it,
      onOpen: onOpen
    }))));
  }
  const cols = mode === 'tiles5' ? 5 : 7;
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%',
      minHeight: 0
    }
  }, /*#__PURE__*/React.createElement(IslandHead, {
    title: "Atalhos",
    count: items.length
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'grid',
      gridTemplateColumns: `repeat(${cols}, minmax(0, 1fr))`,
      gap: 10,
      alignContent: 'start',
      overflow: 'auto',
      minHeight: 0,
      paddingRight: 2
    }
  }, items.map(it => /*#__PURE__*/React.createElement(ShortcutTile, {
    key: it.id,
    name: it.name,
    type: it.type,
    hotkey: it.hotkey,
    style: {
      minWidth: 0
    },
    running: it.running,
    pinned: it.pinned,
    onOpen: () => onOpen(it)
  }))));
}
function ListRow({
  item,
  onOpen
}) {
  const [hover, setHover] = React.useState(false);
  return /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => onOpen(item),
    onMouseEnter: () => setHover(true),
    onMouseLeave: () => setHover(false),
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 9,
      width: '100%',
      height: 34,
      padding: '0 8px',
      border: 'none',
      borderRadius: 'var(--radius-sm)',
      cursor: 'pointer',
      textAlign: 'left',
      background: hover ? 'var(--taskbar-hover)' : 'transparent'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: item.type,
    size: 16,
    color: "var(--accent)"
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      minWidth: 0,
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      color: 'var(--text-primary)',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap'
    }
  }, item.name), item.running && /*#__PURE__*/React.createElement("span", {
    style: {
      width: 6,
      height: 6,
      borderRadius: '50%',
      background: 'var(--success)'
    }
  }), item.hotkey && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--text-tertiary)'
    }
  }, item.hotkey.replace('Ctrl+Alt+', '⌃⌥')));
}
function IslandHead({
  title,
  count,
  action
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'baseline',
      justifyContent: 'space-between',
      marginBottom: 10
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: islandLabel
  }, title, count != null && /*#__PURE__*/React.createElement("span", {
    style: {
      color: 'var(--text-tertiary)',
      opacity: .7
    }
  }, " \xB7 ", count)), action);
}

/* ── mic + áudio ─────────────────────────────────────────────── */
function MicIsland({
  muted,
  onToggle,
  deaf,
  onToggleDeaf
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 8
    }
  }, /*#__PURE__*/React.createElement(PillBtn, {
    active: muted,
    danger: true,
    onClick: onToggle,
    icon: muted ? 'mic-off' : 'mic',
    label: muted ? 'Mutado' : 'Mic'
  }), /*#__PURE__*/React.createElement(PillBtn, {
    active: deaf,
    danger: true,
    onClick: onToggleDeaf,
    icon: "notification",
    label: deaf ? 'Surdo' : 'Áudio'
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 1
    }
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'flex-end',
      gap: 2,
      height: 22
    }
  }, [0.35, 0.7, 1, 0.55, 0.2].map((v, i) => /*#__PURE__*/React.createElement("span", {
    key: i,
    style: {
      width: 3,
      borderRadius: 2,
      height: `${(muted ? 0.12 : v) * 100}%`,
      background: muted ? 'var(--text-tertiary)' : 'var(--success)',
      transition: 'height var(--dur-med) var(--ease-out)'
    }
  }))));
}
function PillBtn({
  icon,
  label,
  active,
  danger,
  onClick
}) {
  const [hover, setHover] = React.useState(false);
  const tone = active && danger ? 'var(--danger)' : 'var(--text-primary)';
  return /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: onClick,
    onMouseEnter: () => setHover(true),
    onMouseLeave: () => setHover(false),
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 7,
      height: 34,
      padding: '0 11px',
      borderRadius: 'var(--radius-sm)',
      cursor: 'pointer',
      color: tone,
      background: active && danger ? 'var(--danger-soft)' : hover ? 'var(--taskbar-hover)' : 'var(--surface-2)',
      border: `1px solid ${active && danger ? 'var(--danger)' : 'var(--border-subtle)'}`,
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      fontWeight: 'var(--fw-medium)'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      width: 16,
      height: 16
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: icon,
    size: "100%"
  })), label);
}

/* ── Discord Rich Presence ───────────────────────────────────── */
function DiscordIsland({
  app,
  tracking,
  onToggle,
  elapsed
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 9
    }
  }, /*#__PURE__*/React.createElement(IslandHead, {
    title: "Discord",
    action: /*#__PURE__*/React.createElement("button", {
      type: "button",
      onClick: onToggle,
      style: {
        width: 34,
        height: 20,
        borderRadius: 'var(--radius-pill)',
        border: 'none',
        cursor: 'pointer',
        padding: 2,
        backgroundColor: tracking ? 'var(--accent)' : 'var(--surface-3)',
        display: 'flex',
        justifyContent: tracking ? 'flex-end' : 'flex-start',
        transition: 'background-color var(--dur-fast)'
      }
    }, /*#__PURE__*/React.createElement("span", {
      style: {
        width: 16,
        height: 16,
        borderRadius: '50%',
        background: '#fff'
      }
    }))
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 10,
      alignItems: 'center'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      width: 44,
      height: 44,
      borderRadius: 'var(--radius-sm)',
      background: 'linear-gradient(150deg,#1f4a73,#132b45)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      flexShrink: 0,
      border: '1px solid var(--border-subtle)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "gamepad",
    size: 22,
    color: tracking ? 'var(--accent)' : 'var(--text-tertiary)'
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      minWidth: 0,
      flex: 1
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-display)',
      fontWeight: 600,
      fontSize: 'var(--fs-subtitle)',
      color: 'var(--text-primary)',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap'
    }
  }, tracking ? app : 'Rastreio desativado'), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 6,
      marginTop: 3
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      width: 6,
      height: 6,
      borderRadius: '50%',
      background: tracking ? 'var(--success)' : 'var(--text-tertiary)'
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--text-secondary)'
    }
  }, tracking ? elapsed : '--:--')))));
}

/* ── performance ─────────────────────────────────────────────── */
function PerfIsland({
  perf
}) {
  return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement(IslandHead, {
    title: "Desempenho"
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 6
    }
  }, [['CPU', perf.cpu, '%'], ['GPU', perf.gpu, '%'], ['FPS', perf.fps, '']].map(([k, v, unit]) => /*#__PURE__*/React.createElement("div", {
    key: k,
    style: {
      ...islandCard,
      flex: 1,
      padding: '7px 9px'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 9.5,
      color: 'var(--text-tertiary)',
      letterSpacing: '.06em'
    }
  }, k), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-display)',
      fontSize: 17,
      fontWeight: 'var(--fw-semibold)',
      color: v > 85 && unit === '%' ? 'var(--danger)' : 'var(--text-primary)',
      lineHeight: 1.2
    }
  }, v, unit), unit === '%' && /*#__PURE__*/React.createElement("div", {
    style: {
      height: 2,
      background: 'var(--surface-3)',
      borderRadius: 1,
      marginTop: 4
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      height: '100%',
      width: `${v}%`,
      background: v > 85 ? 'var(--danger)' : 'var(--accent)',
      borderRadius: 1,
      transition: 'width var(--dur-med) var(--ease-out)'
    }
  }))))));
}

/* ── notificações ────────────────────────────────────────────── */
function NotifIsland({
  items,
  onClear
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      height: '100%',
      minHeight: 0
    }
  }, /*#__PURE__*/React.createElement(IslandHead, {
    title: "Notifica\xE7\xF5es",
    count: items.length,
    action: /*#__PURE__*/React.createElement("button", {
      type: "button",
      onClick: onClear,
      style: {
        background: 'none',
        border: 'none',
        cursor: 'pointer',
        padding: 0,
        fontFamily: 'var(--font-body)',
        fontSize: 'var(--fs-caption)',
        fontWeight: 'var(--fw-medium)',
        color: 'var(--accent)'
      }
    }, "Limpar")
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      flexDirection: 'column',
      gap: 4,
      overflow: 'auto',
      minHeight: 0
    }
  }, items.length === 0 && /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      color: 'var(--text-tertiary)'
    }
  }, "Sem notifica\xE7\xF5es."), items.map((n, i) => /*#__PURE__*/React.createElement("div", {
    key: i,
    style: {
      ...islandCard,
      display: 'flex',
      gap: 8,
      padding: '7px 9px'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: n.icon || 'app',
    size: 15,
    color: "var(--accent)",
    style: {
      marginTop: 1,
      flexShrink: 0
    }
  }), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1,
      minWidth: 0
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'flex',
      justifyContent: 'space-between',
      gap: 6
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-semibold)',
      color: 'var(--text-primary)',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap'
    }
  }, n.title), /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 9.5,
      color: 'var(--text-tertiary)',
      flexShrink: 0
    }
  }, n.time)), /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'block',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      color: 'var(--text-secondary)',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap'
    }
  }, n.body))))));
}

/* ── atalhos rápidos (favoritos) ─────────────────────────────── */
function QuickIsland({
  items,
  onOpen
}) {
  return /*#__PURE__*/React.createElement("div", null, /*#__PURE__*/React.createElement(IslandHead, {
    title: "Favoritos"
  }), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 6
    }
  }, items.slice(0, 5).map(s => /*#__PURE__*/React.createElement(Tooltip, {
    key: s.id,
    label: s.name,
    placement: "bottom"
  }, /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => onOpen(s),
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      width: 42,
      height: 42,
      background: 'var(--surface-2)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-sm)',
      cursor: 'pointer',
      color: 'var(--accent)'
    },
    onMouseEnter: e => e.currentTarget.style.background = 'var(--surface-3)',
    onMouseLeave: e => e.currentTarget.style.background = 'var(--surface-2)'
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      width: 19,
      height: 19
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: s.type,
    size: "100%"
  })))))));
}
Object.assign(window, {
  OVERLAY_SEED,
  SearchIsland,
  GridIsland,
  MicIsland,
  DiscordIsland,
  PerfIsland,
  NotifIsland,
  QuickIsland,
  IslandHead
});
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/overlay/Islands.jsx", error: String((e && e.message) || e) }); }

// ui_kits/overlay/OverlayApp.jsx
try { (() => {
/* Cortinhos — Overlay (tela cheia) exploration.
   Explores replacing the centered launcher window with a Discord-style
   fullscreen overlay: dimmed screen + floating islands the user arranges.
   Stage is a fixed 1440×860 "monitor" scaled to fit the preview. */
const XDS = window.CortinhosDesignSystem_c0acc5;
const {
  Icon,
  Notch,
  Tooltip
} = XDS;
const STAGE_W = 1440,
  STAGE_H = 860;
const STORE_KEY = 'cortinhos.overlay.explore.v2';

/* Island sizes (w, h). h omitted = content height. */
const SIZES = {
  notch: {
    w: 400
  },
  search: {
    w: 460
  },
  grid: {
    w: 860,
    h: 364
  },
  mic: {
    w: 264
  },
  notif: {
    w: 264,
    h: 200
  },
  quick: {
    w: 264
  },
  discord: {
    w: 264
  },
  perf: {
    w: 264
  },
  settings: {
    w: 44,
    h: 44
  }
};

/* Three arrangements. Each also picks how the grid renders, because the
   arrangement and the grid's density are the same decision. */
const PRESETS = {
  A: {
    name: 'Constelação',
    gridMode: 'tiles7',
    blurb: 'Tudo encostado nas bordas, grid como cinturão inferior. Centro da tela livre pro jogo.',
    size: {
      grid: {
        w: 848,
        h: 364
      }
    },
    posIsland: {
      search: {
        x: 24,
        y: 206
      }
    },
    pos: {
      notch: {
        x: 520,
        y: 0
      },
      search: {
        x: 490,
        y: 140
      },
      grid: {
        x: 300,
        y: 478
      },
      mic: {
        x: 24,
        y: 206
      },
      notif: {
        x: 24,
        y: 286
      },
      quick: {
        x: 24,
        y: 502
      },
      discord: {
        x: 1152,
        y: 206
      },
      perf: {
        x: 1152,
        y: 336
      },
      settings: {
        x: 1372,
        y: 24
      }
    }
  },
  B: {
    name: 'Painel direito',
    gridMode: 'tiles5',
    blurb: 'Grid vira um painel alto à direita, notch ancorado à esquerda. Metade esquerda-baixa fica livre.',
    size: {
      grid: {
        w: 686,
        h: 580
      }
    },
    pos: {
      notch: {
        x: 24,
        y: 0
      },
      search: {
        x: 730,
        y: 80
      },
      grid: {
        x: 730,
        y: 150
      },
      mic: {
        x: 24,
        y: 380
      },
      perf: {
        x: 24,
        y: 460
      },
      discord: {
        x: 24,
        y: 574
      },
      notif: {
        x: 24,
        y: 700
      },
      quick: {
        x: 470,
        y: 380
      },
      settings: {
        x: 1372,
        y: 790
      }
    }
  },
  C: {
    name: 'Lista',
    gridMode: 'list',
    blurb: 'Atalhos como lista densa numa coluna estreita. Pega o mínimo de tela — quase todo o jogo visível.',
    size: {
      grid: {
        w: 300,
        h: 568
      }
    },
    pos: {
      notch: {
        x: 520,
        y: 0
      },
      search: {
        x: 24,
        y: 56
      },
      grid: {
        x: 24,
        y: 120
      },
      quick: {
        x: 24,
        y: 676
      },
      mic: {
        x: 1152,
        y: 120
      },
      perf: {
        x: 1152,
        y: 204
      },
      discord: {
        x: 1152,
        y: 320
      },
      notif: {
        x: 1152,
        y: 450
      },
      settings: {
        x: 1372,
        y: 24
      }
    }
  }
};
const SCRIMS = {
  uniforme: {
    label: 'Uniforme',
    desc: 'Scrim sólido + blur leve na tela toda, como o Discord.'
  },
  vinheta: {
    label: 'Vinheta',
    desc: 'Escurece as bordas e deixa o centro do jogo respirando.'
  },
  halo: {
    label: 'Halo local',
    desc: 'Sem scrim global — cada ilha carrega a própria sombra difusa.'
  },
  nenhum: {
    label: 'Nenhum',
    desc: 'Só as ilhas flutuando. Mais leve, menos claro que o overlay está ativo.'
  }
};
const NOTCH_MODES = {
  ilha: {
    label: 'Ilha-mãe',
    desc: 'O notch abre expandido como a ilha superior; mic, perf, Discord e notificações vivem dentro dele.'
  },
  separado: {
    label: 'Separado',
    desc: 'O notch continua sendo a pílula da taskbar; as ilhas são superfícies independentes.'
  },
  oculto: {
    label: 'Oculto',
    desc: 'Com o overlay aberto o notch some — uma superfície de cada vez.'
  }
};
const ABSORBED = ['mic', 'notif', 'perf', 'discord'];

/* ── fake gameplay backdrop (placeholder, not artwork) ────────── */
function GameBackdrop() {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      overflow: 'hidden',
      background: '#0b0d10',
      backgroundImage: 'repeating-linear-gradient(115deg, rgba(255,255,255,.035) 0 2px, transparent 2px 22px), radial-gradient(ellipse 80% 70% at 50% 40%, #1b2431, #0b0d10 70%)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '50%',
      top: '50%',
      transform: 'translate(-50%,-50%)',
      textAlign: 'center',
      fontFamily: 'var(--font-mono)',
      fontSize: 12,
      color: 'rgba(255,255,255,.22)',
      letterSpacing: '.14em',
      textTransform: 'uppercase'
    }
  }, "jogo em execu\xE7\xE3o \xB7 1440 \xD7 860"));
}

/* With the overlay open, the pill stops repeating what the islands show and
   drops to identity + one status dot. Same notch grammar, less to read. */
function IdentityPill({
  alert
}) {
  const [hover, setHover] = React.useState(false);
  return /*#__PURE__*/React.createElement("div", {
    onMouseEnter: () => setHover(true),
    onMouseLeave: () => setHover(false),
    style: {
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 8,
      height: 'var(--taskbar-height)',
      padding: hover ? '0 14px' : '0 12px',
      backgroundColor: 'var(--taskbar-tint)',
      backdropFilter: 'var(--material-acrylic-thin)',
      border: '1px solid var(--border-subtle)',
      borderTop: 'none',
      borderRadius: '0 0 var(--radius-notch) var(--radius-notch)',
      boxShadow: 'var(--elev-2), inset 0 -1px 0 var(--taskbar-hairline)',
      transition: 'padding var(--dur-med) var(--ease-spring)'
    }
  }, /*#__PURE__*/React.createElement("img", {
    src: "../../assets/logo-mini.svg",
    alt: "cortinhos",
    width: "22",
    height: "22",
    style: {
      display: 'block'
    }
  }), hover && /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-medium)',
      color: 'var(--text-secondary)',
      whiteSpace: 'nowrap'
    }
  }, "Overlay ativo"), /*#__PURE__*/React.createElement("span", {
    style: {
      width: 7,
      height: 7,
      borderRadius: '50%',
      flexShrink: 0,
      background: alert ? 'var(--warning)' : 'var(--success)'
    }
  }));
}

/* ── island wrapper: chrome, drag, edit affordances ───────────── */
function Island({
  id,
  label,
  pos,
  size,
  edit,
  scrim,
  onDrag,
  bare,
  children
}) {
  const [drag, setDrag] = React.useState(false);
  const start = React.useRef(null);
  const down = e => {
    if (!edit) return;
    e.preventDefault();
    e.stopPropagation();
    start.current = {
      px: e.clientX,
      py: e.clientY,
      x: pos.x,
      y: pos.y
    };
    setDrag(true);
    e.currentTarget.setPointerCapture(e.pointerId);
  };
  const move = e => {
    if (!drag || !start.current) return;
    const k = window.__overlayScale || 1;
    const nx = start.current.x + (e.clientX - start.current.px) / k;
    const ny = start.current.y + (e.clientY - start.current.py) / k;
    onDrag(id, snap(nx, ny, size));
  };
  const up = e => {
    setDrag(false);
    start.current = null;
    try {
      e.currentTarget.releasePointerCapture(e.pointerId);
    } catch (_) {}
  };
  return /*#__PURE__*/React.createElement("div", {
    "data-island": id,
    onPointerDown: down,
    onPointerMove: move,
    onPointerUp: up,
    onPointerCancel: up,
    style: {
      position: 'absolute',
      left: pos.x,
      top: pos.y,
      width: size.w,
      height: size.h || 'auto',
      boxSizing: 'border-box',
      zIndex: drag ? 40 : 20,
      cursor: edit ? drag ? 'grabbing' : 'grab' : 'default',
      transform: drag ? 'scale(1.02)' : 'scale(1)',
      transition: drag ? 'none' : 'left var(--dur-med) var(--ease-spring), top var(--dur-med) var(--ease-spring), width var(--dur-med) var(--ease-spring), height var(--dur-med) var(--ease-spring)'
    }
  }, scrim === 'halo' && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: -100,
      zIndex: -1,
      pointerEvents: 'none',
      background: 'radial-gradient(ellipse at center, rgba(6,8,11,.85) 0%, rgba(6,8,11,.66) 42%, rgba(6,8,11,0) 74%)'
    }
  }), edit && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: -8,
      borderRadius: 'calc(var(--radius-lg) + 6px)',
      border: `1.5px dashed ${drag ? 'var(--accent)' : 'var(--border-strong)'}`,
      pointerEvents: 'none'
    }
  }), edit && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: -8,
      top: -34,
      display: 'flex',
      alignItems: 'center',
      gap: 4,
      padding: '3px 4px 3px 8px',
      borderRadius: 'var(--radius-sm)',
      background: 'var(--surface-3)',
      border: '1px solid var(--border-subtle)',
      whiteSpace: 'nowrap'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-caption)',
      fontWeight: 'var(--fw-semibold)',
      color: 'var(--text-primary)'
    }
  }, label), /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      width: 20,
      height: 20,
      alignItems: 'center',
      justifyContent: 'center',
      color: 'var(--text-tertiary)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "settings",
    size: 13
  })), /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      width: 20,
      height: 20,
      alignItems: 'center',
      justifyContent: 'center',
      borderRadius: 4,
      background: 'var(--accent)',
      color: 'var(--text-on-accent)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "pin",
    size: 12
  }))), bare ? children : /*#__PURE__*/React.createElement("div", {
    style: {
      height: '100%',
      boxSizing: 'border-box',
      padding: 16,
      background: 'var(--material-acrylic)',
      backdropFilter: 'var(--material-acrylic-blur)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-lg)',
      boxShadow: 'var(--elev-4)',
      color: 'var(--text-primary)',
      display: 'flex',
      flexDirection: 'column',
      minHeight: 0
    }
  }, children));
}
function snap(x, y, size) {
  const M = 24,
    w = size.w,
    h = size.h || 90;
  const cands = [[x, M], [x, STAGE_H - h - M], [M, y], [STAGE_W - w - M, y], [(STAGE_W - w) / 2, y], [x, 0]];
  let nx = x,
    ny = y;
  for (const [cx, cy] of cands) {
    if (Math.abs(cx - x) < 14) nx = cx;
    if (Math.abs(cy - y) < 14) ny = cy;
  }
  nx = Math.max(0, Math.min(STAGE_W - w, Math.round(nx)));
  ny = Math.max(0, Math.min(STAGE_H - h, Math.round(ny)));
  return {
    x: nx,
    y: ny
  };
}

/* ── studio control rail (scaffolding, not product chrome) ────── */
function Seg({
  options,
  value,
  onChange
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 2,
      background: 'rgba(0,0,0,.4)',
      borderRadius: 5,
      padding: 2
    }
  }, options.map(o => /*#__PURE__*/React.createElement("button", {
    key: o.v,
    type: "button",
    onClick: () => onChange(o.v),
    style: {
      padding: '4px 9px',
      borderRadius: 3,
      border: 'none',
      cursor: 'pointer',
      fontFamily: 'var(--font-mono)',
      fontSize: 11,
      background: value === o.v ? 'var(--accent)' : 'transparent',
      color: value === o.v ? 'var(--text-on-accent)' : 'rgba(255,255,255,.62)'
    }
  }, o.l)));
}
function RailRow({
  label,
  children
}) {
  return /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 10,
      justifyContent: 'space-between'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 10,
      letterSpacing: '.1em',
      textTransform: 'uppercase',
      color: 'rgba(255,255,255,.45)'
    }
  }, label), children);
}
function OverlayApp() {
  const saved = React.useMemo(() => {
    try {
      return JSON.parse(localStorage.getItem(STORE_KEY)) || {};
    } catch (_) {
      return {};
    }
  }, []);
  const [preset, setPreset] = React.useState(saved.preset || 'A');
  const [layout, setLayout] = React.useState(saved.layout || null);
  const [scrim, setScrim] = React.useState(saved.scrim || 'vinheta');
  const [notchMode, setNotchMode] = React.useState(saved.notchMode || 'separado');
  const [edit, setEdit] = React.useState(false);
  const [hitViz, setHitViz] = React.useState(false);
  const [open, setOpen] = React.useState(true);
  const [rail, setRail] = React.useState(true);
  const [feas, setFeas] = React.useState(false);
  const [query, setQuery] = React.useState('');
  const [muted, setMuted] = React.useState(false);
  const [deaf, setDeaf] = React.useState(false);
  const [tracking, setTracking] = React.useState(true);
  const [toast, setToast] = React.useState(null);
  const [notifs, setNotifs] = React.useState([{
    title: 'Discord',
    body: 'Luckyx entrou em Sala 3',
    time: 'agora',
    icon: 'app'
  }, {
    title: 'Steam',
    body: 'Download concluído — Mods',
    time: '2 min',
    icon: 'app'
  }, {
    title: 'OBS',
    body: 'Gravação salva em Clipes',
    time: '9 min',
    icon: 'folder'
  }]);
  const [perf, setPerf] = React.useState({
    cpu: 41,
    gpu: 88,
    fps: 144
  });
  const wrapRef = React.useRef(null);
  const [scale, setScale] = React.useState(1);
  const P = PRESETS[preset];
  const base = {
    ...P.pos,
    ...(notchMode === 'ilha' ? P.posIsland || {} : {})
  };
  const pos = layout || base;
  const sizeFor = id => ({
    ...SIZES[id],
    ...((P.size || {})[id] || {})
  });
  React.useEffect(() => {
    const el = wrapRef.current;
    if (!el) return;
    const fit = () => {
      const k = Math.min(el.clientWidth / STAGE_W, el.clientHeight / STAGE_H);
      window.__overlayScale = k;
      setScale(k);
    };
    fit();
    const ro = new ResizeObserver(fit);
    ro.observe(el);
    return () => ro.disconnect();
  }, []);
  React.useEffect(() => {
    localStorage.setItem(STORE_KEY, JSON.stringify({
      preset,
      layout,
      scrim,
      notchMode
    }));
  }, [preset, layout, scrim, notchMode]);
  React.useEffect(() => {
    const t = setInterval(() => setPerf(p => ({
      cpu: Math.max(18, Math.min(97, p.cpu + Math.round((Math.random() - .5) * 12))),
      gpu: Math.max(30, Math.min(99, p.gpu + Math.round((Math.random() - .5) * 10))),
      fps: Math.max(70, Math.min(240, p.fps + Math.round((Math.random() - .5) * 18)))
    })), 1600);
    return () => clearInterval(t);
  }, []);
  React.useEffect(() => {
    const onKey = e => {
      const typing = e.target && /input|textarea/i.test(e.target.tagName);
      if (e.key === 'Escape') {
        setFeas(false);
        setOpen(false);
        return;
      }
      if (typing) return;
      const k = e.key.toLowerCase();
      if (k === 'l') setEdit(v => !v);
      if (k === 'v') setFeas(v => !v);
      if (k === 'h') setRail(v => !v);
      if (k === 'o') setOpen(v => !v);
    };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, []);
  const usePreset = p => {
    setPreset(p);
    setLayout(null);
  };
  const onDrag = (id, next) => setLayout(l => ({
    ...(l || base),
    [id]: next
  }));
  const reset = () => setLayout(null);
  const results = window.OVERLAY_SEED.filter(i => i.name.toLowerCase().includes(query.toLowerCase()));
  const favs = window.OVERLAY_SEED.filter(i => i.pinned);
  const fire = it => {
    setToast(`Abrindo ${it.name}…`);
    setTimeout(() => setToast(null), 1400);
  };
  const absorbed = notchMode === 'ilha';
  const shown = id => !(absorbed && ABSORBED.includes(id));
  const scrimLayer = scrim === 'uniforme' ? {
    background: 'rgba(6,8,11,.62)',
    backdropFilter: 'blur(2px)'
  } : scrim === 'vinheta' ? {
    background: 'radial-gradient(ellipse 62% 55% at 50% 48%, rgba(6,8,11,.10), rgba(6,8,11,.86) 100%)'
  } : null;
  return /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'fixed',
      inset: 0,
      background: '#07090c',
      overflow: 'hidden',
      display: 'flex',
      flexDirection: 'column'
    }
  }, /*#__PURE__*/React.createElement("div", {
    ref: wrapRef,
    style: {
      flex: 1,
      minHeight: 0,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'relative',
      width: STAGE_W,
      height: STAGE_H,
      flex: 'none',
      transform: `scale(${scale})`,
      transformOrigin: 'center center',
      outline: '1px solid rgba(255,255,255,.08)',
      overflow: 'hidden'
    }
  }, /*#__PURE__*/React.createElement(GameBackdrop, null), open && scrimLayer && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      ...scrimLayer,
      transition: 'opacity var(--dur-med) var(--ease-out)'
    }
  }), open && hitViz && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      inset: 0,
      pointerEvents: 'none',
      backgroundImage: 'repeating-linear-gradient(45deg, rgba(34,197,94,.10) 0 6px, transparent 6px 14px)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '50%',
      bottom: 28,
      transform: 'translateX(-50%)',
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      padding: '6px 12px',
      borderRadius: 'var(--radius-pill)',
      background: 'rgba(6,8,11,.8)',
      border: '1px solid var(--success)',
      fontFamily: 'var(--font-mono)',
      fontSize: 11,
      color: 'var(--success)'
    }
  }, "\xE1rea hachurada = clique passa pro jogo")), !open && /*#__PURE__*/React.createElement(React.Fragment, null, /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '50%',
      top: 0,
      transform: 'translateX(-50%)',
      zIndex: 20
    }
  }, /*#__PURE__*/React.createElement(Notch, {
    micMuted: muted,
    discordApp: "Valorant",
    discordTracking: tracking,
    notifications: notifs
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '50%',
      top: '52%',
      transform: 'translate(-50%,-50%)',
      textAlign: 'center',
      fontFamily: 'var(--font-mono)',
      fontSize: 13,
      color: 'rgba(255,255,255,.4)',
      letterSpacing: '.08em'
    }
  }, "overlay fechado \u2014 Ctrl+Alt+L pra abrir")), open && notchMode !== 'oculto' && /*#__PURE__*/React.createElement(Island, {
    id: "notch",
    label: "Notch",
    pos: pos.notch,
    size: sizeFor('notch'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag,
    bare: true
  }, absorbed ? /*#__PURE__*/React.createElement(Notch, {
    state: "expanded",
    onStateChange: () => {},
    micMuted: muted,
    onToggleMic: () => setMuted(m => !m),
    discordApp: "Valorant",
    discordTracking: tracking,
    onToggleTracking: () => setTracking(t => !t),
    quickShortcuts: favs.map(f => ({
      name: f.name,
      type: f.type,
      onOpen: () => fire(f)
    })),
    notifications: notifs,
    onClearNotifications: () => setNotifs([]),
    perf: perf,
    onOpenLauncher: () => setToast('Abrindo tudo…')
  }) : /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      justifyContent: 'center'
    }
  }, /*#__PURE__*/React.createElement(IdentityPill, {
    alert: muted
  }))), open && /*#__PURE__*/React.createElement(Island, {
    id: "search",
    label: "Busca",
    pos: pos.search,
    size: sizeFor('search'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag
  }, /*#__PURE__*/React.createElement(window.SearchIsland, {
    query: query,
    setQuery: setQuery,
    results: results
  })), open && /*#__PURE__*/React.createElement(Island, {
    id: "grid",
    label: "Atalhos",
    pos: pos.grid,
    size: sizeFor('grid'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag
  }, /*#__PURE__*/React.createElement(window.GridIsland, {
    items: results,
    mode: P.gridMode,
    onOpen: fire
  })), open && shown('mic') && /*#__PURE__*/React.createElement(Island, {
    id: "mic",
    label: "Mic + \xE1udio",
    pos: pos.mic,
    size: sizeFor('mic'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag
  }, /*#__PURE__*/React.createElement(window.MicIsland, {
    muted: muted,
    onToggle: () => setMuted(m => !m),
    deaf: deaf,
    onToggleDeaf: () => setDeaf(d => !d)
  })), open && shown('notif') && /*#__PURE__*/React.createElement(Island, {
    id: "notif",
    label: "Notifica\xE7\xF5es",
    pos: pos.notif,
    size: sizeFor('notif'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag
  }, /*#__PURE__*/React.createElement(window.NotifIsland, {
    items: notifs,
    onClear: () => setNotifs([])
  })), open && shown('perf') && /*#__PURE__*/React.createElement(Island, {
    id: "perf",
    label: "Desempenho",
    pos: pos.perf,
    size: sizeFor('perf'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag
  }, /*#__PURE__*/React.createElement(window.PerfIsland, {
    perf: perf
  })), open && shown('discord') && /*#__PURE__*/React.createElement(Island, {
    id: "discord",
    label: "Discord",
    pos: pos.discord,
    size: sizeFor('discord'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag
  }, /*#__PURE__*/React.createElement(window.DiscordIsland, {
    app: "Valorant",
    tracking: tracking,
    onToggle: () => setTracking(t => !t),
    elapsed: "01:24:09"
  })), open && /*#__PURE__*/React.createElement(Island, {
    id: "quick",
    label: "Favoritos",
    pos: pos.quick,
    size: sizeFor('quick'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag
  }, /*#__PURE__*/React.createElement(window.QuickIsland, {
    items: favs,
    onOpen: fire
  })), open && /*#__PURE__*/React.createElement(Island, {
    id: "settings",
    label: "Config",
    pos: pos.settings,
    size: sizeFor('settings'),
    edit: edit,
    scrim: scrim,
    onDrag: onDrag,
    bare: true
  }, /*#__PURE__*/React.createElement("button", {
    type: "button",
    title: "Configura\xE7\xF5es",
    style: {
      width: 44,
      height: 44,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      background: 'var(--material-acrylic)',
      backdropFilter: 'var(--material-acrylic-blur)',
      border: '1px solid var(--border-subtle)',
      borderRadius: 'var(--radius-lg)',
      boxShadow: 'var(--elev-4)',
      cursor: 'pointer',
      color: 'var(--text-secondary)'
    }
  }, /*#__PURE__*/React.createElement(Icon, {
    name: "settings",
    size: 20
  }))), open && edit && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '50%',
      bottom: 24,
      transform: 'translateX(-50%)',
      zIndex: 50,
      display: 'flex',
      alignItems: 'center',
      gap: 12,
      padding: '8px 10px 8px 14px',
      borderRadius: 'var(--radius-pill)',
      background: 'var(--surface-3)',
      border: '1px solid var(--border-strong)',
      boxShadow: 'var(--elev-4)'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      color: 'var(--text-primary)'
    }
  }, "Arraste as ilhas \u2014 elas encaixam nas bordas."), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: reset,
    style: ghostBtn
  }, "Restaurar arranjo"), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => setEdit(false),
    style: {
      ...ghostBtn,
      background: 'var(--accent)',
      color: 'var(--text-on-accent)',
      border: '1px solid transparent'
    }
  }, "Salvar")), toast && /*#__PURE__*/React.createElement("div", {
    style: {
      position: 'absolute',
      left: '50%',
      top: 28,
      transform: 'translateX(-50%)',
      zIndex: 60,
      padding: '8px 14px',
      borderRadius: 'var(--radius-pill)',
      background: 'var(--accent-soft)',
      border: '1px solid var(--accent)',
      color: 'var(--text-primary)',
      fontFamily: 'var(--font-body)',
      fontSize: 'var(--fs-body)',
      fontWeight: 'var(--fw-semibold)'
    }
  }, toast))), rail ? /*#__PURE__*/React.createElement("div", {
    style: {
      flex: 'none',
      display: 'flex',
      flexDirection: 'column',
      gap: 8,
      padding: '10px 16px 12px',
      background: 'rgba(10,13,17,.96)',
      borderTop: '1px dashed rgba(255,255,255,.2)'
    }
  }, /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      alignItems: 'center',
      gap: 18,
      flexWrap: 'wrap'
    }
  }, /*#__PURE__*/React.createElement("span", {
    style: {
      fontFamily: 'var(--font-mono)',
      fontSize: 10,
      letterSpacing: '.14em',
      textTransform: 'uppercase',
      color: 'rgba(255,255,255,.4)'
    }
  }, "explora\xE7\xE3o"), preset === 'A' && scrim === 'vinheta' && notchMode === 'separado' && /*#__PURE__*/React.createElement("span", {
    style: {
      display: 'inline-flex',
      alignItems: 'center',
      gap: 6,
      padding: '3px 9px',
      borderRadius: 'var(--radius-pill)',
      background: 'var(--success-soft)',
      border: '1px solid var(--success)',
      fontFamily: 'var(--font-mono)',
      fontSize: 10,
      color: 'var(--success)'
    }
  }, "dire\xE7\xE3o escolhida"), /*#__PURE__*/React.createElement(RailRow, {
    label: "Arranjo"
  }, /*#__PURE__*/React.createElement(Seg, {
    value: preset,
    onChange: usePreset,
    options: [{
      v: 'A',
      l: 'A'
    }, {
      v: 'B',
      l: 'B'
    }, {
      v: 'C',
      l: 'C'
    }]
  })), /*#__PURE__*/React.createElement(RailRow, {
    label: "Escurecer"
  }, /*#__PURE__*/React.createElement(Seg, {
    value: scrim,
    onChange: setScrim,
    options: [{
      v: 'uniforme',
      l: 'unif'
    }, {
      v: 'vinheta',
      l: 'vinh'
    }, {
      v: 'halo',
      l: 'halo'
    }, {
      v: 'nenhum',
      l: 'off'
    }]
  })), /*#__PURE__*/React.createElement(RailRow, {
    label: "Notch"
  }, /*#__PURE__*/React.createElement(Seg, {
    value: notchMode,
    onChange: setNotchMode,
    options: [{
      v: 'ilha',
      l: 'ilha-mãe'
    }, {
      v: 'separado',
      l: 'sep'
    }, {
      v: 'oculto',
      l: 'off'
    }]
  })), /*#__PURE__*/React.createElement("div", {
    style: {
      display: 'flex',
      gap: 6,
      flexWrap: 'wrap'
    }
  }, /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => setEdit(v => !v),
    style: edit ? activeBtn : ghostBtn
  }, "L \xB7 editar layout"), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => setHitViz(v => !v),
    style: hitViz ? activeBtn : ghostBtn
  }, "click-through"), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => setOpen(v => !v),
    style: ghostBtn
  }, "O \xB7 ", open ? 'fechar' : 'abrir'), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => setFeas(true),
    style: ghostBtn
  }, "V \xB7 viabilidade")), /*#__PURE__*/React.createElement("span", {
    style: {
      flex: 1
    }
  }), /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => setRail(false),
    style: {
      ...ghostBtn,
      padding: '3px 8px',
      fontSize: 10
    }
  }, "H ocultar")), /*#__PURE__*/React.createElement("div", {
    style: {
      fontFamily: 'var(--font-body)',
      fontSize: 11.5,
      lineHeight: 1.5,
      color: 'rgba(255,255,255,.5)',
      textWrap: 'pretty'
    }
  }, /*#__PURE__*/React.createElement("b", {
    style: {
      color: 'rgba(255,255,255,.82)'
    }
  }, P.name), " \u2014 ", P.blurb, " ", /*#__PURE__*/React.createElement("span", {
    style: {
      opacity: .45
    }
  }, "\xB7"), " ", /*#__PURE__*/React.createElement("b", {
    style: {
      color: 'rgba(255,255,255,.82)'
    }
  }, SCRIMS[scrim].label), " \u2014 ", SCRIMS[scrim].desc, " ", /*#__PURE__*/React.createElement("span", {
    style: {
      opacity: .45
    }
  }, "\xB7"), " ", /*#__PURE__*/React.createElement("b", {
    style: {
      color: 'rgba(255,255,255,.82)'
    }
  }, NOTCH_MODES[notchMode].label), " \u2014 ", NOTCH_MODES[notchMode].desc)) : /*#__PURE__*/React.createElement("button", {
    type: "button",
    onClick: () => setRail(true),
    style: {
      ...ghostBtn,
      position: 'fixed',
      left: 16,
      bottom: 16,
      zIndex: 100
    }
  }, "explora\xE7\xE3o"), feas && /*#__PURE__*/React.createElement(window.FeasibilityPanel, {
    onClose: () => setFeas(false)
  }));
}
const ghostBtn = {
  padding: '5px 9px',
  borderRadius: 5,
  cursor: 'pointer',
  background: 'rgba(255,255,255,.06)',
  border: '1px solid rgba(255,255,255,.16)',
  color: 'rgba(255,255,255,.78)',
  fontFamily: 'var(--font-mono)',
  fontSize: 11
};
const activeBtn = {
  ...ghostBtn,
  background: 'var(--accent)',
  color: 'var(--text-on-accent)',
  border: '1px solid transparent'
};
window.OverlayApp = OverlayApp;
})(); } catch (e) { __ds_ns.__errors.push({ path: "ui_kits/overlay/OverlayApp.jsx", error: String((e && e.message) || e) }); }

__ds_ns.Badge = __ds_scope.Badge;

__ds_ns.Icon = __ds_scope.Icon;

__ds_ns.ShortcutTile = __ds_scope.ShortcutTile;

__ds_ns.ContextMenu = __ds_scope.ContextMenu;

__ds_ns.Dialog = __ds_scope.Dialog;

__ds_ns.Tooltip = __ds_scope.Tooltip;

__ds_ns.Button = __ds_scope.Button;

__ds_ns.Checkbox = __ds_scope.Checkbox;

__ds_ns.Select = __ds_scope.Select;

__ds_ns.TextField = __ds_scope.TextField;

__ds_ns.ThemeToggle = __ds_scope.ThemeToggle;

__ds_ns.Notch = __ds_scope.Notch;

})();
