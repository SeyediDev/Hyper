var ni = Object.defineProperty;
var us = (e) => {
  throw TypeError(e);
};
var si = (e, t, r) => t in e ? ni(e, t, { enumerable: !0, configurable: !0, writable: !0, value: r }) : e[t] = r;
var Gr = (e, t, r) => si(e, typeof t != "symbol" ? t + "" : t, r), Yr = (e, t, r) => t.has(e) || us("Cannot " + r);
var v = (e, t, r) => (Yr(e, t, "read from private field"), r ? r.call(e) : t.get(e)), K = (e, t, r) => t.has(e) ? us("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, r), I = (e, t, r, n) => (Yr(e, t, "write to private field"), n ? n.call(e, r) : t.set(e, r), r), ee = (e, t, r) => (Yr(e, t, "access private method"), r);
import * as ae from "react";
import Ln, { useState as G, useCallback as B, useRef as _e, useEffect as me, forwardRef as bt, createContext as ho, useContext as po, useMemo as ke, lazy as In, Suspense as mo } from "react";
import { useLocation as Fn, NavLink as oi, useNavigate as go, Navigate as ai, BrowserRouter as ii, Routes as li, Route as Qe, Outlet as ci } from "react-router-dom";
import { createPortal as Nr } from "react-dom";
import { useDispatch as di, useSelector as ui, Provider as fi } from "react-redux";
var un = { exports: {} }, Nt = {};
/**
 * @license React
 * react-jsx-runtime.production.min.js
 *
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
var fs;
function hi() {
  if (fs) return Nt;
  fs = 1;
  var e = Ln, t = Symbol.for("react.element"), r = Symbol.for("react.fragment"), n = Object.prototype.hasOwnProperty, s = e.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED.ReactCurrentOwner, a = { key: !0, ref: !0, __self: !0, __source: !0 };
  function i(l, u, d) {
    var c, f = {}, y = null, m = null;
    d !== void 0 && (y = "" + d), u.key !== void 0 && (y = "" + u.key), u.ref !== void 0 && (m = u.ref);
    for (c in u) n.call(u, c) && !a.hasOwnProperty(c) && (f[c] = u[c]);
    if (l && l.defaultProps) for (c in u = l.defaultProps, u) f[c] === void 0 && (f[c] = u[c]);
    return { $$typeof: t, type: l, key: y, ref: m, props: f, _owner: s.current };
  }
  return Nt.Fragment = r, Nt.jsx = i, Nt.jsxs = i, Nt;
}
var jt = {};
/**
 * @license React
 * react-jsx-runtime.development.js
 *
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
var hs;
function pi() {
  return hs || (hs = 1, process.env.NODE_ENV !== "production" && function() {
    var e = Ln, t = Symbol.for("react.element"), r = Symbol.for("react.portal"), n = Symbol.for("react.fragment"), s = Symbol.for("react.strict_mode"), a = Symbol.for("react.profiler"), i = Symbol.for("react.provider"), l = Symbol.for("react.context"), u = Symbol.for("react.forward_ref"), d = Symbol.for("react.suspense"), c = Symbol.for("react.suspense_list"), f = Symbol.for("react.memo"), y = Symbol.for("react.lazy"), m = Symbol.for("react.offscreen"), p = Symbol.iterator, b = "@@iterator";
    function g(h) {
      if (h === null || typeof h != "object")
        return null;
      var k = p && h[p] || h[b];
      return typeof k == "function" ? k : null;
    }
    var E = e.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED;
    function N(h) {
      {
        for (var k = arguments.length, O = new Array(k > 1 ? k - 1 : 0), P = 1; P < k; P++)
          O[P - 1] = arguments[P];
        _("error", h, O);
      }
    }
    function _(h, k, O) {
      {
        var P = E.ReactDebugCurrentFrame, q = P.getStackAddendum();
        q !== "" && (k += "%s", O = O.concat([q]));
        var X = O.map(function(H) {
          return String(H);
        });
        X.unshift("Warning: " + k), Function.prototype.apply.call(console[h], console, X);
      }
    }
    var w = !1, S = !1, A = !1, D = !1, U = !1, T;
    T = Symbol.for("react.module.reference");
    function M(h) {
      return !!(typeof h == "string" || typeof h == "function" || h === n || h === a || U || h === s || h === d || h === c || D || h === m || w || S || A || typeof h == "object" && h !== null && (h.$$typeof === y || h.$$typeof === f || h.$$typeof === i || h.$$typeof === l || h.$$typeof === u || // This needs to include all possible module reference object
      // types supported by any Flight configuration anywhere since
      // we don't know which Flight build this will end up being used
      // with.
      h.$$typeof === T || h.getModuleId !== void 0));
    }
    function C(h, k, O) {
      var P = h.displayName;
      if (P)
        return P;
      var q = k.displayName || k.name || "";
      return q !== "" ? O + "(" + q + ")" : O;
    }
    function $(h) {
      return h.displayName || "Context";
    }
    function R(h) {
      if (h == null)
        return null;
      if (typeof h.tag == "number" && N("Received an unexpected object in getComponentNameFromType(). This is likely a bug in React. Please file an issue."), typeof h == "function")
        return h.displayName || h.name || null;
      if (typeof h == "string")
        return h;
      switch (h) {
        case n:
          return "Fragment";
        case r:
          return "Portal";
        case a:
          return "Profiler";
        case s:
          return "StrictMode";
        case d:
          return "Suspense";
        case c:
          return "SuspenseList";
      }
      if (typeof h == "object")
        switch (h.$$typeof) {
          case l:
            var k = h;
            return $(k) + ".Consumer";
          case i:
            var O = h;
            return $(O._context) + ".Provider";
          case u:
            return C(h, h.render, "ForwardRef");
          case f:
            var P = h.displayName || null;
            return P !== null ? P : R(h.type) || "Memo";
          case y: {
            var q = h, X = q._payload, H = q._init;
            try {
              return R(H(X));
            } catch {
              return null;
            }
          }
        }
      return null;
    }
    var V = Object.assign, L = 0, Y, Te, ue, Et, Kt, Ne, De;
    function at() {
    }
    at.__reactDisabledLog = !0;
    function it() {
      {
        if (L === 0) {
          Y = console.log, Te = console.info, ue = console.warn, Et = console.error, Kt = console.group, Ne = console.groupCollapsed, De = console.groupEnd;
          var h = {
            configurable: !0,
            enumerable: !0,
            value: at,
            writable: !0
          };
          Object.defineProperties(console, {
            info: h,
            log: h,
            warn: h,
            error: h,
            group: h,
            groupCollapsed: h,
            groupEnd: h
          });
        }
        L++;
      }
    }
    function Jt() {
      {
        if (L--, L === 0) {
          var h = {
            configurable: !0,
            enumerable: !0,
            writable: !0
          };
          Object.defineProperties(console, {
            log: V({}, h, {
              value: Y
            }),
            info: V({}, h, {
              value: Te
            }),
            warn: V({}, h, {
              value: ue
            }),
            error: V({}, h, {
              value: Et
            }),
            group: V({}, h, {
              value: Kt
            }),
            groupCollapsed: V({}, h, {
              value: Ne
            }),
            groupEnd: V({}, h, {
              value: De
            })
          });
        }
        L < 0 && N("disabledDepth fell below zero. This is a bug in React. Please file an issue.");
      }
    }
    var Vr = E.ReactCurrentDispatcher, zr;
    function Qt(h, k, O) {
      {
        if (zr === void 0)
          try {
            throw Error();
          } catch (q) {
            var P = q.stack.trim().match(/\n( *(at )?)/);
            zr = P && P[1] || "";
          }
        return `
` + zr + h;
      }
    }
    var Wr = !1, Gt;
    {
      var Aa = typeof WeakMap == "function" ? WeakMap : Map;
      Gt = new Aa();
    }
    function Gn(h, k) {
      if (!h || Wr)
        return "";
      {
        var O = Gt.get(h);
        if (O !== void 0)
          return O;
      }
      var P;
      Wr = !0;
      var q = Error.prepareStackTrace;
      Error.prepareStackTrace = void 0;
      var X;
      X = Vr.current, Vr.current = null, it();
      try {
        if (k) {
          var H = function() {
            throw Error();
          };
          if (Object.defineProperty(H.prototype, "props", {
            set: function() {
              throw Error();
            }
          }), typeof Reflect == "object" && Reflect.construct) {
            try {
              Reflect.construct(H, []);
            } catch (fe) {
              P = fe;
            }
            Reflect.construct(h, [], H);
          } else {
            try {
              H.call();
            } catch (fe) {
              P = fe;
            }
            h.call(H.prototype);
          }
        } else {
          try {
            throw Error();
          } catch (fe) {
            P = fe;
          }
          h();
        }
      } catch (fe) {
        if (fe && P && typeof fe.stack == "string") {
          for (var z = fe.stack.split(`
`), le = P.stack.split(`
`), re = z.length - 1, ne = le.length - 1; re >= 1 && ne >= 0 && z[re] !== le[ne]; )
            ne--;
          for (; re >= 1 && ne >= 0; re--, ne--)
            if (z[re] !== le[ne]) {
              if (re !== 1 || ne !== 1)
                do
                  if (re--, ne--, ne < 0 || z[re] !== le[ne]) {
                    var Ee = `
` + z[re].replace(" at new ", " at ");
                    return h.displayName && Ee.includes("<anonymous>") && (Ee = Ee.replace("<anonymous>", h.displayName)), typeof h == "function" && Gt.set(h, Ee), Ee;
                  }
                while (re >= 1 && ne >= 0);
              break;
            }
        }
      } finally {
        Wr = !1, Vr.current = X, Jt(), Error.prepareStackTrace = q;
      }
      var ct = h ? h.displayName || h.name : "", Ke = ct ? Qt(ct) : "";
      return typeof h == "function" && Gt.set(h, Ke), Ke;
    }
    function Da(h, k, O) {
      return Gn(h, !1);
    }
    function Ma(h) {
      var k = h.prototype;
      return !!(k && k.isReactComponent);
    }
    function Yt(h, k, O) {
      if (h == null)
        return "";
      if (typeof h == "function")
        return Gn(h, Ma(h));
      if (typeof h == "string")
        return Qt(h);
      switch (h) {
        case d:
          return Qt("Suspense");
        case c:
          return Qt("SuspenseList");
      }
      if (typeof h == "object")
        switch (h.$$typeof) {
          case u:
            return Da(h.render);
          case f:
            return Yt(h.type, k, O);
          case y: {
            var P = h, q = P._payload, X = P._init;
            try {
              return Yt(X(q), k, O);
            } catch {
            }
          }
        }
      return "";
    }
    var kt = Object.prototype.hasOwnProperty, Yn = {}, Xn = E.ReactDebugCurrentFrame;
    function Xt(h) {
      if (h) {
        var k = h._owner, O = Yt(h.type, h._source, k ? k.type : null);
        Xn.setExtraStackFrame(O);
      } else
        Xn.setExtraStackFrame(null);
    }
    function Pa(h, k, O, P, q) {
      {
        var X = Function.call.bind(kt);
        for (var H in h)
          if (X(h, H)) {
            var z = void 0;
            try {
              if (typeof h[H] != "function") {
                var le = Error((P || "React class") + ": " + O + " type `" + H + "` is invalid; it must be a function, usually from the `prop-types` package, but received `" + typeof h[H] + "`.This often happens because of typos such as `PropTypes.function` instead of `PropTypes.func`.");
                throw le.name = "Invariant Violation", le;
              }
              z = h[H](k, H, P, O, null, "SECRET_DO_NOT_PASS_THIS_OR_YOU_WILL_BE_FIRED");
            } catch (re) {
              z = re;
            }
            z && !(z instanceof Error) && (Xt(q), N("%s: type specification of %s `%s` is invalid; the type checker function must return `null` or an `Error` but returned a %s. You may have forgotten to pass an argument to the type checker creator (arrayOf, instanceOf, objectOf, oneOf, oneOfType, and shape all require an argument).", P || "React class", O, H, typeof z), Xt(null)), z instanceof Error && !(z.message in Yn) && (Yn[z.message] = !0, Xt(q), N("Failed %s type: %s", O, z.message), Xt(null));
          }
      }
    }
    var La = Array.isArray;
    function Hr(h) {
      return La(h);
    }
    function Ia(h) {
      {
        var k = typeof Symbol == "function" && Symbol.toStringTag, O = k && h[Symbol.toStringTag] || h.constructor.name || "Object";
        return O;
      }
    }
    function Fa(h) {
      try {
        return Zn(h), !1;
      } catch {
        return !0;
      }
    }
    function Zn(h) {
      return "" + h;
    }
    function es(h) {
      if (Fa(h))
        return N("The provided key is an unsupported type %s. This value must be coerced to a string before before using it here.", Ia(h)), Zn(h);
    }
    var ts = E.ReactCurrentOwner, Ua = {
      key: !0,
      ref: !0,
      __self: !0,
      __source: !0
    }, rs, ns;
    function $a(h) {
      if (kt.call(h, "ref")) {
        var k = Object.getOwnPropertyDescriptor(h, "ref").get;
        if (k && k.isReactWarning)
          return !1;
      }
      return h.ref !== void 0;
    }
    function Ba(h) {
      if (kt.call(h, "key")) {
        var k = Object.getOwnPropertyDescriptor(h, "key").get;
        if (k && k.isReactWarning)
          return !1;
      }
      return h.key !== void 0;
    }
    function Va(h, k) {
      typeof h.ref == "string" && ts.current;
    }
    function za(h, k) {
      {
        var O = function() {
          rs || (rs = !0, N("%s: `key` is not a prop. Trying to access it will result in `undefined` being returned. If you need to access the same value within the child component, you should pass it as a different prop. (https://reactjs.org/link/special-props)", k));
        };
        O.isReactWarning = !0, Object.defineProperty(h, "key", {
          get: O,
          configurable: !0
        });
      }
    }
    function Wa(h, k) {
      {
        var O = function() {
          ns || (ns = !0, N("%s: `ref` is not a prop. Trying to access it will result in `undefined` being returned. If you need to access the same value within the child component, you should pass it as a different prop. (https://reactjs.org/link/special-props)", k));
        };
        O.isReactWarning = !0, Object.defineProperty(h, "ref", {
          get: O,
          configurable: !0
        });
      }
    }
    var Ha = function(h, k, O, P, q, X, H) {
      var z = {
        // This tag allows us to uniquely identify this as a React Element
        $$typeof: t,
        // Built-in properties that belong on the element
        type: h,
        key: k,
        ref: O,
        props: H,
        // Record the component responsible for creating this element.
        _owner: X
      };
      return z._store = {}, Object.defineProperty(z._store, "validated", {
        configurable: !1,
        enumerable: !1,
        writable: !0,
        value: !1
      }), Object.defineProperty(z, "_self", {
        configurable: !1,
        enumerable: !1,
        writable: !1,
        value: P
      }), Object.defineProperty(z, "_source", {
        configurable: !1,
        enumerable: !1,
        writable: !1,
        value: q
      }), Object.freeze && (Object.freeze(z.props), Object.freeze(z)), z;
    };
    function qa(h, k, O, P, q) {
      {
        var X, H = {}, z = null, le = null;
        O !== void 0 && (es(O), z = "" + O), Ba(k) && (es(k.key), z = "" + k.key), $a(k) && (le = k.ref, Va(k, q));
        for (X in k)
          kt.call(k, X) && !Ua.hasOwnProperty(X) && (H[X] = k[X]);
        if (h && h.defaultProps) {
          var re = h.defaultProps;
          for (X in re)
            H[X] === void 0 && (H[X] = re[X]);
        }
        if (z || le) {
          var ne = typeof h == "function" ? h.displayName || h.name || "Unknown" : h;
          z && za(H, ne), le && Wa(H, ne);
        }
        return Ha(h, z, le, q, P, ts.current, H);
      }
    }
    var qr = E.ReactCurrentOwner, ss = E.ReactDebugCurrentFrame;
    function lt(h) {
      if (h) {
        var k = h._owner, O = Yt(h.type, h._source, k ? k.type : null);
        ss.setExtraStackFrame(O);
      } else
        ss.setExtraStackFrame(null);
    }
    var Kr;
    Kr = !1;
    function Jr(h) {
      return typeof h == "object" && h !== null && h.$$typeof === t;
    }
    function os() {
      {
        if (qr.current) {
          var h = R(qr.current.type);
          if (h)
            return `

Check the render method of \`` + h + "`.";
        }
        return "";
      }
    }
    function Ka(h) {
      return "";
    }
    var as = {};
    function Ja(h) {
      {
        var k = os();
        if (!k) {
          var O = typeof h == "string" ? h : h.displayName || h.name;
          O && (k = `

Check the top-level render call using <` + O + ">.");
        }
        return k;
      }
    }
    function is(h, k) {
      {
        if (!h._store || h._store.validated || h.key != null)
          return;
        h._store.validated = !0;
        var O = Ja(k);
        if (as[O])
          return;
        as[O] = !0;
        var P = "";
        h && h._owner && h._owner !== qr.current && (P = " It was passed a child from " + R(h._owner.type) + "."), lt(h), N('Each child in a list should have a unique "key" prop.%s%s See https://reactjs.org/link/warning-keys for more information.', O, P), lt(null);
      }
    }
    function ls(h, k) {
      {
        if (typeof h != "object")
          return;
        if (Hr(h))
          for (var O = 0; O < h.length; O++) {
            var P = h[O];
            Jr(P) && is(P, k);
          }
        else if (Jr(h))
          h._store && (h._store.validated = !0);
        else if (h) {
          var q = g(h);
          if (typeof q == "function" && q !== h.entries)
            for (var X = q.call(h), H; !(H = X.next()).done; )
              Jr(H.value) && is(H.value, k);
        }
      }
    }
    function Qa(h) {
      {
        var k = h.type;
        if (k == null || typeof k == "string")
          return;
        var O;
        if (typeof k == "function")
          O = k.propTypes;
        else if (typeof k == "object" && (k.$$typeof === u || // Note: Memo only checks outer props here.
        // Inner props are checked in the reconciler.
        k.$$typeof === f))
          O = k.propTypes;
        else
          return;
        if (O) {
          var P = R(k);
          Pa(O, h.props, "prop", P, h);
        } else if (k.PropTypes !== void 0 && !Kr) {
          Kr = !0;
          var q = R(k);
          N("Component %s declared `PropTypes` instead of `propTypes`. Did you misspell the property assignment?", q || "Unknown");
        }
        typeof k.getDefaultProps == "function" && !k.getDefaultProps.isReactClassApproved && N("getDefaultProps is only used on classic React.createClass definitions. Use a static property named `defaultProps` instead.");
      }
    }
    function Ga(h) {
      {
        for (var k = Object.keys(h.props), O = 0; O < k.length; O++) {
          var P = k[O];
          if (P !== "children" && P !== "key") {
            lt(h), N("Invalid prop `%s` supplied to `React.Fragment`. React.Fragment can only have `key` and `children` props.", P), lt(null);
            break;
          }
        }
        h.ref !== null && (lt(h), N("Invalid attribute `ref` supplied to `React.Fragment`."), lt(null));
      }
    }
    var cs = {};
    function ds(h, k, O, P, q, X) {
      {
        var H = M(h);
        if (!H) {
          var z = "";
          (h === void 0 || typeof h == "object" && h !== null && Object.keys(h).length === 0) && (z += " You likely forgot to export your component from the file it's defined in, or you might have mixed up default and named imports.");
          var le = Ka();
          le ? z += le : z += os();
          var re;
          h === null ? re = "null" : Hr(h) ? re = "array" : h !== void 0 && h.$$typeof === t ? (re = "<" + (R(h.type) || "Unknown") + " />", z = " Did you accidentally export a JSX literal instead of a component?") : re = typeof h, N("React.jsx: type is invalid -- expected a string (for built-in components) or a class/function (for composite components) but got: %s.%s", re, z);
        }
        var ne = qa(h, k, O, q, X);
        if (ne == null)
          return ne;
        if (H) {
          var Ee = k.children;
          if (Ee !== void 0)
            if (P)
              if (Hr(Ee)) {
                for (var ct = 0; ct < Ee.length; ct++)
                  ls(Ee[ct], h);
                Object.freeze && Object.freeze(Ee);
              } else
                N("React.jsx: Static children should always be an array. You are likely explicitly calling React.jsxs or React.jsxDEV. Use the Babel transform instead.");
            else
              ls(Ee, h);
        }
        if (kt.call(k, "key")) {
          var Ke = R(h), fe = Object.keys(k).filter(function(ri) {
            return ri !== "key";
          }), Qr = fe.length > 0 ? "{key: someKey, " + fe.join(": ..., ") + ": ...}" : "{key: someKey}";
          if (!cs[Ke + Qr]) {
            var ti = fe.length > 0 ? "{" + fe.join(": ..., ") + ": ...}" : "{}";
            N(`A props object containing a "key" prop is being spread into JSX:
  let props = %s;
  <%s {...props} />
React keys must be passed directly to JSX without using spread:
  let props = %s;
  <%s key={someKey} {...props} />`, Qr, Ke, ti, Ke), cs[Ke + Qr] = !0;
          }
        }
        return h === n ? Ga(ne) : Qa(ne), ne;
      }
    }
    function Ya(h, k, O) {
      return ds(h, k, O, !0);
    }
    function Xa(h, k, O) {
      return ds(h, k, O, !1);
    }
    var Za = Xa, ei = Ya;
    jt.Fragment = n, jt.jsx = Za, jt.jsxs = ei;
  }()), jt;
}
process.env.NODE_ENV === "production" ? un.exports = hi() : un.exports = pi();
var o = un.exports;
function yo(e) {
  var t, r, n = "";
  if (typeof e == "string" || typeof e == "number") n += e;
  else if (typeof e == "object") if (Array.isArray(e)) {
    var s = e.length;
    for (t = 0; t < s; t++) e[t] && (r = yo(e[t])) && (n && (n += " "), n += r);
  } else for (r in e) e[r] && (n && (n += " "), n += r);
  return n;
}
function j() {
  for (var e, t, r = 0, n = "", s = arguments.length; r < s; r++) (e = arguments[r]) && (t = yo(e)) && (n && (n += " "), n += t);
  return n;
}
const mi = ({
  items: e,
  collapsed: t = !1,
  onCollapsedChange: r,
  logo: n,
  logoText: s = "Admin Panel",
  footer: a,
  className: i,
  hasPermission: l = () => !0
}) => {
  const u = Fn(), [d, c] = G(/* @__PURE__ */ new Set()), f = B((p) => {
    c((b) => {
      const g = new Set(b);
      return g.has(p) ? g.delete(p) : g.add(p), g;
    });
  }, []), y = B((p) => p ? u.pathname === p || u.pathname.startsWith(p + "/") : !1, [u.pathname]), m = (p, b = 0) => {
    if (p.permission && !l(p.permission))
      return null;
    if (p.divider)
      return /* @__PURE__ */ o.jsx(
        "div",
        {
          className: "my-2 mx-4 border-t border-gray-200 dark:border-gray-700"
        },
        p.id
      );
    const g = p.children && p.children.length > 0, E = d.has(p.id), N = y(p.path), _ = /* @__PURE__ */ o.jsxs(o.Fragment, { children: [
      p.icon && /* @__PURE__ */ o.jsx("span", { className: "flex-shrink-0 w-5 h-5 flex items-center justify-center", children: p.icon }),
      !t && /* @__PURE__ */ o.jsx("span", { className: "flex-1 text-sm font-medium truncate", children: p.label }),
      !t && p.badge && /* @__PURE__ */ o.jsx(
        "span",
        {
          className: j(
            "px-2 py-0.5 text-xs font-semibold rounded-full",
            {
              "bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-300": p.badgeColor === "primary",
              "bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300": p.badgeColor === "success",
              "bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-300": p.badgeColor === "warning",
              "bg-red-100 text-red-700 dark:bg-red-900 dark:text-red-300": p.badgeColor === "danger",
              "bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300": p.badgeColor === "info",
              "bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-300": !p.badgeColor
            }
          ),
          children: p.badge
        }
      ),
      !t && g && /* @__PURE__ */ o.jsx(
        "svg",
        {
          className: j(
            "w-4 h-4 transition-transform duration-200",
            E && "rotate-90"
          ),
          fill: "none",
          stroke: "currentColor",
          viewBox: "0 0 24 24",
          children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M9 5l7 7-7 7" })
        }
      )
    ] }), w = j(
      "flex items-center gap-3 px-4 py-2.5 rounded-lg transition-all duration-200",
      "hover:bg-gray-100 dark:hover:bg-gray-800",
      N && "bg-blue-50 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400",
      !N && "text-gray-700 dark:text-gray-300",
      t && "justify-center px-2",
      b > 0 && !t && "mr-4"
    );
    return /* @__PURE__ */ o.jsxs("div", { children: [
      p.path && !g ? /* @__PURE__ */ o.jsx(oi, { to: p.path, className: w, title: t ? p.label : void 0, children: _ }) : /* @__PURE__ */ o.jsx(
        "button",
        {
          onClick: () => g && f(p.id),
          className: j(w, "w-full text-right"),
          title: t ? p.label : void 0,
          children: _
        }
      ),
      g && E && !t && /* @__PURE__ */ o.jsx("div", { className: "mt-1 space-y-1", children: p.children.map((S) => m(S, b + 1)) })
    ] }, p.id);
  };
  return /* @__PURE__ */ o.jsxs(
    "aside",
    {
      className: j(
        "flex flex-col h-screen bg-white dark:bg-gray-900 border-l border-gray-200 dark:border-gray-700",
        "transition-all duration-300 ease-in-out",
        t ? "w-16" : "w-64",
        i
      ),
      children: [
        /* @__PURE__ */ o.jsxs("div", { className: "flex items-center justify-between h-16 px-4 border-b border-gray-200 dark:border-gray-700", children: [
          !t && /* @__PURE__ */ o.jsxs("div", { className: "flex items-center gap-3", children: [
            typeof n == "string" ? /* @__PURE__ */ o.jsx("img", { src: n, alt: "Logo", className: "w-8 h-8" }) : n,
            /* @__PURE__ */ o.jsx("span", { className: "font-bold text-gray-900 dark:text-white", children: s })
          ] }),
          /* @__PURE__ */ o.jsx(
            "button",
            {
              onClick: () => r == null ? void 0 : r(!t),
              className: j(
                "p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800",
                "text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200",
                t && "mx-auto"
              ),
              title: t ? "باز کردن منو" : "بستن منو",
              children: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: t ? /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M13 5l7 7-7 7M5 5l7 7-7 7" }) : /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M11 19l-7-7 7-7m8 14l-7-7 7-7" }) })
            }
          )
        ] }),
        /* @__PURE__ */ o.jsx("nav", { className: "flex-1 overflow-y-auto p-2 space-y-1", children: e.map((p) => m(p)) }),
        a && /* @__PURE__ */ o.jsx("div", { className: "border-t border-gray-200 dark:border-gray-700 p-4", children: a })
      ]
    }
  );
}, gi = ({
  user: e,
  breadcrumbs: t = [],
  notifications: r = [],
  unreadCount: n = 0,
  theme: s = "light",
  onThemeChange: a,
  onLogout: i,
  onMenuToggle: l,
  onNotificationClick: u,
  onViewAllNotifications: d,
  actions: c,
  className: f,
  showMenuToggle: y = !0
}) => {
  const [m, p] = G(!1), [b, g] = G(!1), E = _e(null), N = _e(null);
  me(() => {
    const w = (S) => {
      E.current && !E.current.contains(S.target) && p(!1), N.current && !N.current.contains(S.target) && g(!1);
    };
    return document.addEventListener("mousedown", w), () => document.removeEventListener("mousedown", w);
  }, []);
  const _ = (w) => {
    switch (w) {
      case "success":
        return /* @__PURE__ */ o.jsx("span", { className: "w-8 h-8 rounded-full bg-green-100 flex items-center justify-center", children: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4 text-green-500", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M5 13l4 4L19 7" }) }) });
      case "warning":
        return /* @__PURE__ */ o.jsx("span", { className: "w-8 h-8 rounded-full bg-yellow-100 flex items-center justify-center", children: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4 text-yellow-500", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" }) }) });
      case "error":
        return /* @__PURE__ */ o.jsx("span", { className: "w-8 h-8 rounded-full bg-red-100 flex items-center justify-center", children: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4 text-red-500", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M6 18L18 6M6 6l12 12" }) }) });
      default:
        return /* @__PURE__ */ o.jsx("span", { className: "w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center", children: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4 text-blue-500", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" }) }) });
    }
  };
  return /* @__PURE__ */ o.jsxs(
    "header",
    {
      className: j(
        "h-16 bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-700",
        "flex items-center justify-between px-4",
        f
      ),
      children: [
        /* @__PURE__ */ o.jsxs("div", { className: "flex items-center gap-4", children: [
          y && /* @__PURE__ */ o.jsx(
            "button",
            {
              onClick: l,
              className: "p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-500 dark:text-gray-400",
              children: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M4 6h16M4 12h16M4 18h16" }) })
            }
          ),
          t.length > 0 && /* @__PURE__ */ o.jsx("nav", { className: "flex items-center gap-2 text-sm", children: t.map((w, S) => /* @__PURE__ */ o.jsxs(Ln.Fragment, { children: [
            S > 0 && /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4 text-gray-400", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M15 19l-7-7 7-7" }) }),
            w.path ? /* @__PURE__ */ o.jsx(
              "a",
              {
                href: w.path,
                className: "text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200",
                children: w.label
              }
            ) : /* @__PURE__ */ o.jsx("span", { className: "text-gray-900 dark:text-white font-medium", children: w.label })
          ] }, S)) })
        ] }),
        /* @__PURE__ */ o.jsxs("div", { className: "flex items-center gap-3", children: [
          c,
          a && /* @__PURE__ */ o.jsx(
            "button",
            {
              onClick: () => a(s === "light" ? "dark" : "light"),
              className: "p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-500 dark:text-gray-400",
              title: s === "light" ? "حالت تاریک" : "حالت روشن",
              children: s === "light" ? /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" }) }) : /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" }) })
            }
          ),
          /* @__PURE__ */ o.jsxs("div", { ref: N, className: "relative", children: [
            /* @__PURE__ */ o.jsxs(
              "button",
              {
                onClick: () => g(!b),
                className: "p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-500 dark:text-gray-400 relative",
                children: [
                  /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" }) }),
                  n > 0 && /* @__PURE__ */ o.jsx("span", { className: "absolute top-1 right-1 w-4 h-4 bg-red-500 text-white text-xs rounded-full flex items-center justify-center", children: n > 9 ? "9+" : n })
                ]
              }
            ),
            b && /* @__PURE__ */ o.jsxs("div", { className: "absolute left-0 mt-2 w-80 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 z-50", children: [
              /* @__PURE__ */ o.jsx("div", { className: "p-3 border-b border-gray-200 dark:border-gray-700", children: /* @__PURE__ */ o.jsx("h3", { className: "font-semibold text-gray-900 dark:text-white", children: "اعلان‌ها" }) }),
              /* @__PURE__ */ o.jsx("div", { className: "max-h-80 overflow-y-auto", children: r.length === 0 ? /* @__PURE__ */ o.jsx("div", { className: "p-4 text-center text-gray-500 dark:text-gray-400", children: "اعلانی وجود ندارد" }) : r.map((w) => /* @__PURE__ */ o.jsxs(
                "button",
                {
                  onClick: () => u == null ? void 0 : u(w),
                  className: j(
                    "w-full p-3 flex items-start gap-3 hover:bg-gray-50 dark:hover:bg-gray-700 text-right",
                    !w.read && "bg-blue-50 dark:bg-blue-900/20"
                  ),
                  children: [
                    _(w.type),
                    /* @__PURE__ */ o.jsxs("div", { className: "flex-1 min-w-0", children: [
                      /* @__PURE__ */ o.jsx("p", { className: "text-sm font-medium text-gray-900 dark:text-white truncate", children: w.title }),
                      /* @__PURE__ */ o.jsx("p", { className: "text-xs text-gray-500 dark:text-gray-400 line-clamp-2", children: w.message }),
                      /* @__PURE__ */ o.jsx("p", { className: "text-xs text-gray-400 dark:text-gray-500 mt-1", children: w.time })
                    ] })
                  ]
                },
                w.id
              )) }),
              r.length > 0 && /* @__PURE__ */ o.jsx("div", { className: "p-2 border-t border-gray-200 dark:border-gray-700", children: /* @__PURE__ */ o.jsx(
                "button",
                {
                  onClick: d,
                  className: "w-full py-2 text-sm text-blue-600 hover:text-blue-700 dark:text-blue-400",
                  children: "مشاهده همه اعلان‌ها"
                }
              ) })
            ] })
          ] }),
          e && /* @__PURE__ */ o.jsxs("div", { ref: E, className: "relative", children: [
            /* @__PURE__ */ o.jsxs(
              "button",
              {
                onClick: () => p(!m),
                className: "flex items-center gap-2 p-1.5 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800",
                children: [
                  e.avatar ? /* @__PURE__ */ o.jsx("img", { src: e.avatar, alt: e.name, className: "w-8 h-8 rounded-full" }) : /* @__PURE__ */ o.jsx("div", { className: "w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white font-medium", children: e.name.charAt(0) }),
                  /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4 text-gray-400", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M19 9l-7 7-7-7" }) })
                ]
              }
            ),
            m && /* @__PURE__ */ o.jsxs("div", { className: "absolute left-0 mt-2 w-56 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 z-50", children: [
              /* @__PURE__ */ o.jsxs("div", { className: "p-3 border-b border-gray-200 dark:border-gray-700", children: [
                /* @__PURE__ */ o.jsx("p", { className: "font-medium text-gray-900 dark:text-white", children: e.name }),
                e.email && /* @__PURE__ */ o.jsx("p", { className: "text-sm text-gray-500 dark:text-gray-400", children: e.email }),
                e.role && /* @__PURE__ */ o.jsx("span", { className: "inline-block mt-1 px-2 py-0.5 bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300 text-xs rounded", children: e.role })
              ] }),
              /* @__PURE__ */ o.jsx("div", { className: "p-2", children: /* @__PURE__ */ o.jsxs(
                "button",
                {
                  onClick: i,
                  className: "w-full flex items-center gap-2 px-3 py-2 text-sm text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-lg",
                  children: [
                    /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" }) }),
                    "خروج از حساب"
                  ]
                }
              ) })
            ] })
          ] })
        ] })
      ]
    }
  );
}, yi = ({
  children: e,
  menuItems: t,
  user: r,
  breadcrumbs: n,
  notifications: s,
  unreadCount: a,
  logo: i,
  logoText: l,
  sidebarFooter: u,
  headerActions: d,
  theme: c = "light",
  onThemeChange: f,
  onLogout: y,
  onNotificationClick: m,
  onViewAllNotifications: p,
  hasPermission: b,
  defaultCollapsed: g = !1,
  className: E,
  sidebarClassName: N,
  headerClassName: _,
  contentClassName: w
}) => {
  const [S, A] = G(g), [D, U] = G(!1), T = B((C) => {
    A(C);
  }, []), M = B(() => {
    window.innerWidth < 1024 ? U((C) => !C) : A((C) => !C);
  }, []);
  return /* @__PURE__ */ o.jsxs(
    "div",
    {
      className: j(
        "flex h-screen bg-gray-100 dark:bg-gray-950",
        c === "dark" && "dark",
        E
      ),
      children: [
        D && /* @__PURE__ */ o.jsx(
          "div",
          {
            className: "fixed inset-0 bg-black/50 z-40 lg:hidden",
            onClick: () => U(!1)
          }
        ),
        /* @__PURE__ */ o.jsx(
          "div",
          {
            className: j(
              "fixed lg:static inset-y-0 right-0 z-50 lg:z-auto",
              "transform lg:transform-none transition-transform duration-300",
              D ? "translate-x-0" : "translate-x-full lg:translate-x-0"
            ),
            children: /* @__PURE__ */ o.jsx(
              mi,
              {
                items: t,
                collapsed: S,
                onCollapsedChange: T,
                logo: i,
                logoText: l,
                footer: u,
                hasPermission: b,
                className: N
              }
            )
          }
        ),
        /* @__PURE__ */ o.jsxs("div", { className: "flex-1 flex flex-col min-w-0", children: [
          /* @__PURE__ */ o.jsx(
            gi,
            {
              user: r,
              breadcrumbs: n,
              notifications: s,
              unreadCount: a,
              theme: c,
              onThemeChange: f,
              onLogout: y,
              onMenuToggle: M,
              onNotificationClick: m,
              onViewAllNotifications: p,
              actions: d,
              className: _
            }
          ),
          /* @__PURE__ */ o.jsx(
            "main",
            {
              className: j(
                "flex-1 overflow-auto p-6",
                w
              ),
              children: e
            }
          )
        ] })
      ]
    }
  );
}, xi = {
  primary: j(
    "bg-gradient-to-r from-purple-600 to-pink-600",
    "hover:from-purple-500 hover:to-pink-500",
    "text-white shadow-lg shadow-purple-500/25",
    "focus:ring-purple-500"
  ),
  secondary: j(
    "bg-gray-100 dark:bg-gray-800",
    "hover:bg-gray-200 dark:hover:bg-gray-700",
    "text-gray-900 dark:text-white",
    "focus:ring-gray-500"
  ),
  outline: j(
    "border-2 border-gray-300 dark:border-gray-600",
    "hover:border-purple-500 dark:hover:border-purple-400",
    "hover:bg-purple-50 dark:hover:bg-purple-900/20",
    "text-gray-700 dark:text-gray-300",
    "focus:ring-purple-500"
  ),
  ghost: j(
    "hover:bg-gray-100 dark:hover:bg-gray-800",
    "text-gray-700 dark:text-gray-300",
    "focus:ring-gray-500"
  ),
  danger: j(
    "bg-red-600 hover:bg-red-500",
    "text-white shadow-lg shadow-red-500/25",
    "focus:ring-red-500"
  ),
  success: j(
    "bg-emerald-600 hover:bg-emerald-500",
    "text-white shadow-lg shadow-emerald-500/25",
    "focus:ring-emerald-500"
  )
}, bi = {
  xs: "px-2.5 py-1 text-xs rounded-md gap-1",
  sm: "px-3 py-1.5 text-sm rounded-lg gap-1.5",
  md: "px-4 py-2 text-sm rounded-lg gap-2",
  lg: "px-5 py-2.5 text-base rounded-xl gap-2",
  xl: "px-6 py-3 text-lg rounded-xl gap-2.5"
}, vi = {
  xs: "p-1 rounded-md",
  sm: "p-1.5 rounded-lg",
  md: "p-2 rounded-lg",
  lg: "p-2.5 rounded-xl",
  xl: "p-3 rounded-xl"
}, wi = bt(
  ({
    variant: e = "primary",
    size: t = "md",
    fullWidth: r = !1,
    loading: n = !1,
    leftIcon: s,
    rightIcon: a,
    iconOnly: i = !1,
    disabled: l,
    className: u,
    children: d,
    ...c
  }, f) => {
    const y = l || n;
    return /* @__PURE__ */ o.jsxs(
      "button",
      {
        ref: f,
        disabled: y,
        className: j(
          // Base styles
          "inline-flex items-center justify-center font-medium",
          "transition-all duration-200",
          "focus:outline-none focus:ring-2 focus:ring-offset-2 dark:focus:ring-offset-gray-900",
          // Variant
          xi[e],
          // Size
          i ? vi[t] : bi[t],
          // Width
          r && "w-full",
          // Disabled
          y && "opacity-60 cursor-not-allowed",
          // Custom
          u
        ),
        ...c,
        children: [
          n && /* @__PURE__ */ o.jsxs(
            "svg",
            {
              className: "animate-spin h-4 w-4",
              fill: "none",
              viewBox: "0 0 24 24",
              children: [
                /* @__PURE__ */ o.jsx(
                  "circle",
                  {
                    className: "opacity-25",
                    cx: "12",
                    cy: "12",
                    r: "10",
                    stroke: "currentColor",
                    strokeWidth: "4"
                  }
                ),
                /* @__PURE__ */ o.jsx(
                  "path",
                  {
                    className: "opacity-75",
                    fill: "currentColor",
                    d: "M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                  }
                )
              ]
            }
          ),
          !n && s && /* @__PURE__ */ o.jsx("span", { className: "flex-shrink-0", children: s }),
          d && /* @__PURE__ */ o.jsx("span", { children: d }),
          a && /* @__PURE__ */ o.jsx("span", { className: "flex-shrink-0", children: a })
        ]
      }
    );
  }
);
wi.displayName = "Button";
const Ei = {
  sm: {
    input: "px-3 py-1.5 text-sm rounded-lg",
    label: "text-xs mb-1",
    helper: "text-xs mt-1"
  },
  md: {
    input: "px-4 py-2.5 text-sm rounded-xl",
    label: "text-sm mb-1.5",
    helper: "text-xs mt-1.5"
  },
  lg: {
    input: "px-5 py-3 text-base rounded-xl",
    label: "text-base mb-2",
    helper: "text-sm mt-2"
  }
}, ki = bt(
  ({
    label: e,
    helperText: t,
    error: r,
    size: n = "md",
    leftIcon: s,
    rightIcon: a,
    fullWidth: i = !1,
    showPasswordToggle: l = !1,
    type: u = "text",
    disabled: d,
    className: c,
    id: f,
    ...y
  }, m) => {
    const [p, b] = G(!1), g = f || `input-${Math.random().toString(36).substr(2, 9)}`, E = u === "password", N = E && p ? "text" : u, _ = Ei[n];
    return /* @__PURE__ */ o.jsxs("div", { className: j(i && "w-full", c), children: [
      e && /* @__PURE__ */ o.jsx(
        "label",
        {
          htmlFor: g,
          className: j(
            "block font-medium text-gray-700 dark:text-gray-300",
            _.label
          ),
          children: e
        }
      ),
      /* @__PURE__ */ o.jsxs("div", { className: "relative", children: [
        s && /* @__PURE__ */ o.jsx("div", { className: "absolute right-3 top-1/2 -translate-y-1/2 text-gray-400", children: s }),
        /* @__PURE__ */ o.jsx(
          "input",
          {
            ref: m,
            id: g,
            type: N,
            disabled: d,
            className: j(
              // Base
              "w-full",
              "bg-white dark:bg-gray-800",
              "border transition-all duration-200",
              "placeholder-gray-400 dark:placeholder-gray-500",
              "text-gray-900 dark:text-white",
              "focus:outline-none focus:ring-2 focus:ring-offset-0",
              // Size
              _.input,
              // Left icon padding
              s && "pr-10",
              // Right icon/toggle padding
              (a || E && l) && "pl-10",
              // State
              r ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : "border-gray-300 dark:border-gray-600 focus:border-purple-500 focus:ring-purple-500/20",
              // Disabled
              d && "opacity-60 cursor-not-allowed bg-gray-100 dark:bg-gray-900"
            ),
            ...y
          }
        ),
        /* @__PURE__ */ o.jsxs("div", { className: "absolute left-3 top-1/2 -translate-y-1/2 flex items-center gap-2", children: [
          E && l && /* @__PURE__ */ o.jsx(
            "button",
            {
              type: "button",
              onClick: () => b(!p),
              className: "text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors",
              tabIndex: -1,
              children: p ? /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" }) }) : /* @__PURE__ */ o.jsxs("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: [
                /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M15 12a3 3 0 11-6 0 3 3 0 016 0z" }),
                /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" })
              ] })
            }
          ),
          a && /* @__PURE__ */ o.jsx("span", { className: "text-gray-400", children: a })
        ] })
      ] }),
      (t || r) && /* @__PURE__ */ o.jsx(
        "p",
        {
          className: j(
            _.helper,
            r ? "text-red-500" : "text-gray-500 dark:text-gray-400"
          ),
          children: r || t
        }
      )
    ] });
  }
);
ki.displayName = "Input";
const Ni = {
  none: "",
  sm: "p-4",
  md: "p-6",
  lg: "p-8"
}, ji = {
  default: "bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700",
  bordered: "bg-white dark:bg-gray-800 border-2 border-gray-300 dark:border-gray-600",
  elevated: "bg-white dark:bg-gray-800 shadow-xl shadow-gray-200/50 dark:shadow-gray-900/50",
  gradient: "bg-gradient-to-br from-white to-gray-50 dark:from-gray-800 dark:to-gray-900 border border-gray-200 dark:border-gray-700"
}, Si = bt(
  ({
    variant: e = "default",
    padding: t = "md",
    hoverable: r = !1,
    clickable: n = !1,
    className: s,
    children: a,
    ...i
  }, l) => /* @__PURE__ */ o.jsx(
    "div",
    {
      ref: l,
      className: j(
        "rounded-2xl",
        ji[e],
        Ni[t],
        r && "transition-all duration-200 hover:shadow-lg hover:-translate-y-0.5",
        n && "cursor-pointer",
        s
      ),
      ...i,
      children: a
    }
  )
);
Si.displayName = "Card";
const _i = bt(
  ({ title: e, subtitle: t, action: r, className: n, children: s, ...a }, i) => /* @__PURE__ */ o.jsxs(
    "div",
    {
      ref: i,
      className: j(
        "flex items-start justify-between gap-4 mb-4",
        n
      ),
      ...a,
      children: [
        /* @__PURE__ */ o.jsxs("div", { children: [
          e && /* @__PURE__ */ o.jsx("h3", { className: "text-lg font-semibold text-gray-900 dark:text-white", children: e }),
          t && /* @__PURE__ */ o.jsx("p", { className: "text-sm text-gray-500 dark:text-gray-400 mt-0.5", children: t }),
          s
        ] }),
        r && /* @__PURE__ */ o.jsx("div", { className: "flex-shrink-0", children: r })
      ]
    }
  )
);
_i.displayName = "CardHeader";
const Ri = bt(
  ({ className: e, children: t, ...r }, n) => /* @__PURE__ */ o.jsx("div", { ref: n, className: j("", e), ...r, children: t })
);
Ri.displayName = "CardBody";
const Oi = bt(
  ({ align: e = "right", className: t, children: r, ...n }, s) => {
    const a = {
      left: "justify-start",
      center: "justify-center",
      right: "justify-end",
      between: "justify-between"
    };
    return /* @__PURE__ */ o.jsx(
      "div",
      {
        ref: s,
        className: j(
          "flex items-center gap-3 mt-4 pt-4 border-t border-gray-200 dark:border-gray-700",
          a[e],
          t
        ),
        ...n,
        children: r
      }
    );
  }
);
Oi.displayName = "CardFooter";
const Ti = {
  sm: "max-w-sm",
  md: "max-w-md",
  lg: "max-w-lg",
  xl: "max-w-xl",
  full: "max-w-[90vw] max-h-[90vh]"
};
function Ci({
  isOpen: e,
  onClose: t,
  title: r,
  description: n,
  children: s,
  footer: a,
  size: i = "md",
  closeOnBackdrop: l = !0,
  closeOnEscape: u = !0,
  showCloseButton: d = !0,
  centered: c = !0,
  className: f,
  overlayClassName: y
}) {
  const m = B(
    (g) => {
      g.key === "Escape" && u && t();
    },
    [u, t]
  ), p = B(
    (g) => {
      g.target === g.currentTarget && l && t();
    },
    [l, t]
  );
  if (me(() => (e && (document.addEventListener("keydown", m), document.body.style.overflow = "hidden"), () => {
    document.removeEventListener("keydown", m), document.body.style.overflow = "";
  }), [e, m]), !e) return null;
  const b = /* @__PURE__ */ o.jsxs(
    "div",
    {
      className: j(
        "fixed inset-0 z-50",
        "flex p-4",
        c ? "items-center" : "items-start pt-20",
        "justify-center",
        y
      ),
      onClick: p,
      role: "dialog",
      "aria-modal": "true",
      "aria-labelledby": r ? "modal-title" : void 0,
      children: [
        /* @__PURE__ */ o.jsx("div", { className: "fixed inset-0 bg-black/50 backdrop-blur-sm", "aria-hidden": "true" }),
        /* @__PURE__ */ o.jsxs(
          "div",
          {
            className: j(
              "relative w-full",
              Ti[i],
              "bg-white dark:bg-gray-800",
              "rounded-2xl shadow-2xl",
              "transform transition-all duration-200",
              "animate-in fade-in zoom-in-95",
              f
            ),
            children: [
              (r || d) && /* @__PURE__ */ o.jsxs("div", { className: "flex items-start justify-between p-6 pb-0", children: [
                /* @__PURE__ */ o.jsxs("div", { children: [
                  r && /* @__PURE__ */ o.jsx(
                    "h2",
                    {
                      id: "modal-title",
                      className: "text-xl font-semibold text-gray-900 dark:text-white",
                      children: r
                    }
                  ),
                  n && /* @__PURE__ */ o.jsx("p", { className: "mt-1 text-sm text-gray-500 dark:text-gray-400", children: n })
                ] }),
                d && /* @__PURE__ */ o.jsx(
                  "button",
                  {
                    onClick: t,
                    className: j(
                      "p-2 rounded-lg -mt-1 -ml-1",
                      "text-gray-400 hover:text-gray-600 dark:hover:text-gray-300",
                      "hover:bg-gray-100 dark:hover:bg-gray-700",
                      "transition-colors duration-200"
                    ),
                    "aria-label": "بستن",
                    children: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M6 18L18 6M6 6l12 12" }) })
                  }
                )
              ] }),
              /* @__PURE__ */ o.jsx("div", { className: "p-6 max-h-[70vh] overflow-y-auto", children: s }),
              a && /* @__PURE__ */ o.jsx("div", { className: "flex items-center justify-end gap-3 p-6 pt-0 border-t border-gray-200 dark:border-gray-700", children: a })
            ]
          }
        )
      ]
    }
  );
  return Nr(b, document.body);
}
const xo = ho(null);
function Ju() {
  const e = po(xo);
  if (!e)
    throw new Error("useToast must be used within ToastProvider");
  return e;
}
function Qu({
  children: e,
  position: t = "top-right",
  maxToasts: r = 5,
  defaultDuration: n = 5e3
}) {
  const [s, a] = G([]), i = B(
    (m) => {
      const p = `toast-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`, b = {
        ...m,
        id: p,
        duration: m.duration ?? n
      };
      return a((g) => [b, ...g].slice(0, r)), p;
    },
    [n, r]
  ), l = B((m) => {
    a((p) => p.filter((b) => b.id !== m));
  }, []), u = B(
    (m, p) => i({ type: "success", title: m, message: p }),
    [i]
  ), d = B(
    (m, p) => i({ type: "error", title: m, message: p }),
    [i]
  ), c = B(
    (m, p) => i({ type: "warning", title: m, message: p }),
    [i]
  ), f = B(
    (m, p) => i({ type: "info", title: m, message: p }),
    [i]
  ), y = {
    toasts: s,
    addToast: i,
    removeToast: l,
    success: u,
    error: d,
    warning: c,
    info: f
  };
  return /* @__PURE__ */ o.jsxs(xo.Provider, { value: y, children: [
    e,
    /* @__PURE__ */ o.jsx(Ai, { toasts: s, position: t, onRemove: l })
  ] });
}
function Ai({ toasts: e, position: t, onRemove: r }) {
  if (e.length === 0) return null;
  const n = {
    "top-right": "top-4 right-4",
    "top-left": "top-4 left-4",
    "bottom-right": "bottom-4 right-4",
    "bottom-left": "bottom-4 left-4",
    "top-center": "top-4 left-1/2 -translate-x-1/2",
    "bottom-center": "bottom-4 left-1/2 -translate-x-1/2"
  }, s = /* @__PURE__ */ o.jsx(
    "div",
    {
      className: j(
        "fixed z-[100] flex flex-col gap-2 max-w-sm w-full",
        n[t]
      ),
      children: e.map((a) => /* @__PURE__ */ o.jsx(Di, { toast: a, onRemove: r }, a.id))
    }
  );
  return Nr(s, document.body);
}
function Di({ toast: e, onRemove: t }) {
  const [r, n] = G(!1);
  me(() => {
    if (e.duration && e.duration > 0) {
      const l = setTimeout(() => {
        n(!0), setTimeout(() => t(e.id), 200);
      }, e.duration);
      return () => clearTimeout(l);
    }
  }, [e.id, e.duration, t]);
  const s = () => {
    n(!0), setTimeout(() => t(e.id), 200);
  }, i = {
    success: {
      icon: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M5 13l4 4L19 7" }) }),
      colors: "bg-emerald-500 text-white"
    },
    error: {
      icon: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M6 18L18 6M6 6l12 12" }) }),
      colors: "bg-red-500 text-white"
    },
    warning: {
      icon: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" }) }),
      colors: "bg-amber-500 text-white"
    },
    info: {
      icon: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" }) }),
      colors: "bg-blue-500 text-white"
    }
  }[e.type];
  return /* @__PURE__ */ o.jsxs(
    "div",
    {
      className: j(
        "flex items-start gap-3 p-4 rounded-xl shadow-lg",
        "bg-white dark:bg-gray-800",
        "border border-gray-200 dark:border-gray-700",
        "transform transition-all duration-200",
        r ? "opacity-0 translate-x-4" : "opacity-100 translate-x-0"
      ),
      children: [
        /* @__PURE__ */ o.jsx("div", { className: j("flex-shrink-0 w-8 h-8 rounded-lg flex items-center justify-center", i.colors), children: i.icon }),
        /* @__PURE__ */ o.jsxs("div", { className: "flex-1 min-w-0", children: [
          /* @__PURE__ */ o.jsx("p", { className: "font-medium text-gray-900 dark:text-white", children: e.title }),
          e.message && /* @__PURE__ */ o.jsx("p", { className: "mt-0.5 text-sm text-gray-500 dark:text-gray-400", children: e.message }),
          e.action && /* @__PURE__ */ o.jsx(
            "button",
            {
              onClick: e.action.onClick,
              className: "mt-2 text-sm font-medium text-purple-600 hover:text-purple-500",
              children: e.action.label
            }
          )
        ] }),
        /* @__PURE__ */ o.jsx(
          "button",
          {
            onClick: s,
            className: "flex-shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300",
            children: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M6 18L18 6M6 6l12 12" }) })
          }
        )
      ]
    }
  );
}
function St(e, t) {
  return typeof t == "function" ? t(e) : e[t];
}
function Xr(e, t) {
  return t ? typeof t == "function" ? t(e) : e[t] : null;
}
function Mi({ columns: e }) {
  return /* @__PURE__ */ o.jsx(o.Fragment, { children: [1, 2, 3, 4, 5].map((t) => /* @__PURE__ */ o.jsx("tr", { className: "animate-pulse", children: Array.from({ length: e }).map((r, n) => /* @__PURE__ */ o.jsx("td", { className: "px-4 py-3", children: /* @__PURE__ */ o.jsx("div", { className: "h-4 bg-gray-200 dark:bg-gray-700 rounded" }) }, n)) }, t)) });
}
function Pi({ message: e }) {
  return /* @__PURE__ */ o.jsx("tr", { children: /* @__PURE__ */ o.jsx("td", { colSpan: 100, className: "px-4 py-12 text-center", children: /* @__PURE__ */ o.jsxs("div", { className: "text-gray-400 dark:text-gray-500", children: [
    /* @__PURE__ */ o.jsx("svg", { className: "w-12 h-12 mx-auto mb-3 opacity-50", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 1.5, d: "M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" }) }),
    /* @__PURE__ */ o.jsx("p", { children: e || "داده‌ای یافت نشد" })
  ] }) }) });
}
function Gu({
  data: e,
  columns: t,
  rowKey: r,
  loading: n = !1,
  emptyMessage: s,
  striped: a = !1,
  hoverable: i = !0,
  compact: l = !1,
  bordered: u = !1,
  stickyHeader: d = !1,
  selectable: c = !1,
  selectedKeys: f = /* @__PURE__ */ new Set(),
  onSelectionChange: y,
  onRowClick: m,
  className: p,
  maxHeight: b
}) {
  const [g, E] = G(null), [N, _] = G("asc"), w = ke(
    () => t.filter((C) => !C.hidden),
    [t]
  ), S = B((C) => {
    g === C ? _(($) => $ === "asc" ? "desc" : "asc") : (E(C), _("asc"));
  }, [g]), A = ke(() => {
    if (!g) return e;
    const C = t.find(($) => $.key === g);
    return C ? [...e].sort(($, R) => {
      const V = Xr($, C.accessor), L = Xr(R, C.accessor);
      if (V === L) return 0;
      if (V == null) return 1;
      if (L == null) return -1;
      const Y = V < L ? -1 : 1;
      return N === "asc" ? Y : -Y;
    }) : e;
  }, [e, t, g, N]), D = e.length > 0 && e.every((C) => f.has(St(C, r))), U = e.some((C) => f.has(St(C, r))), T = B(() => {
    if (D)
      y == null || y(/* @__PURE__ */ new Set());
    else {
      const C = new Set(e.map(($) => St($, r)));
      y == null || y(C);
    }
  }, [D, e, r, y]), M = B((C) => {
    const $ = St(C, r), R = new Set(f);
    R.has($) ? R.delete($) : R.add($), y == null || y(R);
  }, [r, f, y]);
  return /* @__PURE__ */ o.jsx(
    "div",
    {
      className: j(
        "overflow-auto rounded-xl border border-gray-200 dark:border-gray-700",
        p
      ),
      style: b ? { maxHeight: b } : void 0,
      children: /* @__PURE__ */ o.jsxs("table", { className: "w-full text-sm", children: [
        /* @__PURE__ */ o.jsx(
          "thead",
          {
            className: j(
              "bg-gray-50 dark:bg-gray-800/50 text-gray-600 dark:text-gray-400",
              d && "sticky top-0 z-10"
            ),
            children: /* @__PURE__ */ o.jsxs("tr", { children: [
              c && /* @__PURE__ */ o.jsx("th", { className: "w-12 px-4 py-3", children: /* @__PURE__ */ o.jsx(
                "input",
                {
                  type: "checkbox",
                  checked: D,
                  ref: (C) => {
                    C && (C.indeterminate = U && !D);
                  },
                  onChange: T,
                  className: "w-4 h-4 rounded border-gray-300 text-purple-600 focus:ring-purple-500"
                }
              ) }),
              w.map((C) => /* @__PURE__ */ o.jsx(
                "th",
                {
                  className: j(
                    "px-4 font-medium",
                    l ? "py-2" : "py-3",
                    u && "border-l border-gray-200 dark:border-gray-700 first:border-l-0",
                    C.sortable && "cursor-pointer select-none hover:bg-gray-100 dark:hover:bg-gray-700",
                    C.align === "center" && "text-center",
                    C.align === "right" && "text-left",
                    !C.align && "text-right"
                  ),
                  style: C.width ? { width: C.width } : void 0,
                  onClick: () => C.sortable && S(C.key),
                  children: /* @__PURE__ */ o.jsxs("div", { className: "flex items-center gap-1", children: [
                    /* @__PURE__ */ o.jsx("span", { children: C.header }),
                    C.sortable && g === C.key && /* @__PURE__ */ o.jsx(
                      "svg",
                      {
                        className: j(
                          "w-4 h-4 transition-transform",
                          N === "desc" && "rotate-180"
                        ),
                        fill: "none",
                        stroke: "currentColor",
                        viewBox: "0 0 24 24",
                        children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M5 15l7-7 7 7" })
                      }
                    )
                  ] })
                },
                C.key
              ))
            ] })
          }
        ),
        /* @__PURE__ */ o.jsx("tbody", { className: "bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700", children: n ? /* @__PURE__ */ o.jsx(Mi, { columns: w.length + (c ? 1 : 0) }) : A.length === 0 ? /* @__PURE__ */ o.jsx(Pi, { message: s }) : A.map((C, $) => {
          const R = St(C, r), V = f.has(R);
          return /* @__PURE__ */ o.jsxs(
            "tr",
            {
              onClick: () => m == null ? void 0 : m(C, $),
              className: j(
                "transition-colors",
                a && $ % 2 === 1 && "bg-gray-50/50 dark:bg-gray-900/20",
                i && "hover:bg-gray-50 dark:hover:bg-gray-700/50",
                m && "cursor-pointer",
                V && "bg-purple-50 dark:bg-purple-900/20"
              ),
              children: [
                c && /* @__PURE__ */ o.jsx("td", { className: "w-12 px-4 py-3", children: /* @__PURE__ */ o.jsx(
                  "input",
                  {
                    type: "checkbox",
                    checked: V,
                    onChange: (L) => {
                      L.stopPropagation(), M(C);
                    },
                    onClick: (L) => L.stopPropagation(),
                    className: "w-4 h-4 rounded border-gray-300 text-purple-600 focus:ring-purple-500"
                  }
                ) }),
                w.map((L) => {
                  const Y = Xr(C, L.accessor), Te = L.render ? L.render(Y, C, $) : String(Y ?? "-");
                  return /* @__PURE__ */ o.jsx(
                    "td",
                    {
                      className: j(
                        "px-4 text-gray-900 dark:text-gray-100",
                        l ? "py-2" : "py-3",
                        u && "border-l border-gray-200 dark:border-gray-700 first:border-l-0",
                        L.align === "center" && "text-center",
                        L.align === "right" && "text-left",
                        !L.align && "text-right"
                      ),
                      style: L.width ? { width: L.width } : void 0,
                      children: Te
                    },
                    L.key
                  );
                })
              ]
            },
            R
          );
        }) })
      ] })
    }
  );
}
const Li = {
  default: "bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-300",
  primary: "bg-purple-100 text-purple-700 dark:bg-purple-900/50 dark:text-purple-300",
  success: "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/50 dark:text-emerald-300",
  warning: "bg-amber-100 text-amber-700 dark:bg-amber-900/50 dark:text-amber-300",
  danger: "bg-red-100 text-red-700 dark:bg-red-900/50 dark:text-red-300",
  info: "bg-blue-100 text-blue-700 dark:bg-blue-900/50 dark:text-blue-300"
}, Ii = {
  default: "bg-gray-500",
  primary: "bg-purple-500",
  success: "bg-emerald-500",
  warning: "bg-amber-500",
  danger: "bg-red-500",
  info: "bg-blue-500"
}, Fi = {
  sm: "px-2 py-0.5 text-xs",
  md: "px-2.5 py-1 text-xs",
  lg: "px-3 py-1.5 text-sm"
};
function Yu({
  variant: e = "default",
  size: t = "md",
  dot: r = !1,
  pulse: n = !1,
  icon: s,
  removable: a = !1,
  onRemove: i,
  className: l,
  children: u,
  ...d
}) {
  return r ? /* @__PURE__ */ o.jsx(
    "span",
    {
      className: j(
        "inline-flex w-2.5 h-2.5 rounded-full",
        Ii[e],
        n && "animate-pulse",
        l
      ),
      ...d
    }
  ) : /* @__PURE__ */ o.jsxs(
    "span",
    {
      className: j(
        "inline-flex items-center gap-1 font-medium rounded-full",
        Li[e],
        Fi[t],
        l
      ),
      ...d,
      children: [
        s && /* @__PURE__ */ o.jsx("span", { className: "flex-shrink-0", children: s }),
        u,
        a && /* @__PURE__ */ o.jsx(
          "button",
          {
            type: "button",
            onClick: (c) => {
              c.stopPropagation(), i == null || i();
            },
            className: "flex-shrink-0 -mr-1 hover:opacity-75 transition-opacity",
            children: /* @__PURE__ */ o.jsx("svg", { className: "w-3 h-3", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M6 18L18 6M6 6l12 12" }) })
          }
        )
      ]
    }
  );
}
const bo = {
  xs: { container: "w-6 h-6", text: "text-xs", indicator: "w-1.5 h-1.5" },
  sm: { container: "w-8 h-8", text: "text-sm", indicator: "w-2 h-2" },
  md: { container: "w-10 h-10", text: "text-base", indicator: "w-2.5 h-2.5" },
  lg: { container: "w-12 h-12", text: "text-lg", indicator: "w-3 h-3" },
  xl: { container: "w-16 h-16", text: "text-xl", indicator: "w-3.5 h-3.5" },
  "2xl": { container: "w-20 h-20", text: "text-2xl", indicator: "w-4 h-4" }
};
function Ui(e) {
  const t = [
    "bg-red-500",
    "bg-orange-500",
    "bg-amber-500",
    "bg-yellow-500",
    "bg-lime-500",
    "bg-green-500",
    "bg-emerald-500",
    "bg-teal-500",
    "bg-cyan-500",
    "bg-sky-500",
    "bg-blue-500",
    "bg-indigo-500",
    "bg-violet-500",
    "bg-purple-500",
    "bg-fuchsia-500",
    "bg-pink-500"
  ];
  let r = 0;
  for (let n = 0; n < e.length; n++)
    r = e.charCodeAt(n) + ((r << 5) - r);
  return t[Math.abs(r) % t.length];
}
function $i(e) {
  const t = e.trim().split(/\s+/);
  return t.length >= 2 ? `${t[0][0]}${t[t.length - 1][0]}`.toUpperCase() : e.slice(0, 2).toUpperCase();
}
function Xu({
  src: e,
  name: t,
  size: r = "md",
  shape: n = "circle",
  online: s,
  color: a,
  className: i,
  alt: l,
  ...u
}) {
  const [d, c] = G(!1), f = bo[r], y = e && !d, m = t ? $i(t) : "?", p = a || (t ? Ui(t) : "bg-gray-400");
  return /* @__PURE__ */ o.jsxs("div", { className: j("relative inline-flex flex-shrink-0", i), children: [
    /* @__PURE__ */ o.jsx(
      "div",
      {
        className: j(
          "flex items-center justify-center overflow-hidden",
          f.container,
          n === "circle" ? "rounded-full" : "rounded-lg",
          !y && p,
          !y && "text-white font-medium"
        ),
        children: y ? /* @__PURE__ */ o.jsx(
          "img",
          {
            src: e,
            alt: l || t || "Avatar",
            onError: () => c(!0),
            className: "w-full h-full object-cover",
            ...u
          }
        ) : /* @__PURE__ */ o.jsx("span", { className: f.text, children: m })
      }
    ),
    s !== void 0 && /* @__PURE__ */ o.jsx(
      "span",
      {
        className: j(
          "absolute bottom-0 left-0 block rounded-full ring-2 ring-white dark:ring-gray-800",
          f.indicator,
          s ? "bg-emerald-500" : "bg-gray-400"
        )
      }
    )
  ] });
}
function Zu({
  max: e = 4,
  size: t = "md",
  children: r,
  className: n
}) {
  const s = Array.isArray(r) ? r : [r], a = s.slice(0, e), i = s.length - e, l = bo[t];
  return /* @__PURE__ */ o.jsxs("div", { className: j("flex -space-x-2 rtl:space-x-reverse", n), children: [
    a.map((u, d) => /* @__PURE__ */ o.jsx(
      "div",
      {
        className: "ring-2 ring-white dark:ring-gray-800 rounded-full",
        style: { zIndex: a.length - d },
        children: u
      },
      d
    )),
    i > 0 && /* @__PURE__ */ o.jsxs(
      "div",
      {
        className: j(
          "flex items-center justify-center rounded-full",
          "bg-gray-200 dark:bg-gray-700",
          "text-gray-600 dark:text-gray-300",
          "ring-2 ring-white dark:ring-gray-800",
          l.container,
          l.text,
          "font-medium"
        ),
        children: [
          "+",
          i
        ]
      }
    )
  ] });
}
function Bi({
  isOpen: e,
  items: t,
  onItemClick: r,
  onClose: n,
  triggerRef: s,
  placement: a,
  minWidth: i
}) {
  const l = _e(null), [u, d] = G({ top: 0, left: 0 });
  return me(() => {
    var E, N;
    if (!e || !s.current) return;
    const f = s.current.getBoundingClientRect(), y = ((E = l.current) == null ? void 0 : E.offsetHeight) || 200;
    let m = 0, p = 0;
    a.startsWith("bottom") ? m = f.bottom + 8 : m = f.top - y - 8, a.endsWith("start") ? p = f.left : p = f.right - (((N = l.current) == null ? void 0 : N.offsetWidth) || i);
    const b = window.innerHeight, g = window.innerWidth;
    m + y > b && (m = f.top - y - 8), p < 0 && (p = 8), p + i > g && (p = g - i - 8), d({ top: m, left: p });
  }, [e, s, a, i]), me(() => {
    if (!e) return;
    const c = (y) => {
      l.current && !l.current.contains(y.target) && s.current && !s.current.contains(y.target) && n();
    }, f = (y) => {
      y.key === "Escape" && n();
    };
    return document.addEventListener("mousedown", c), document.addEventListener("keydown", f), () => {
      document.removeEventListener("mousedown", c), document.removeEventListener("keydown", f);
    };
  }, [e, n, s]), e ? Nr(
    /* @__PURE__ */ o.jsx(
      "div",
      {
        ref: l,
        className: j(
          "fixed z-50 py-1",
          "bg-white dark:bg-gray-800",
          "border border-gray-200 dark:border-gray-700",
          "rounded-xl shadow-lg",
          "animate-in fade-in zoom-in-95 duration-150"
        ),
        style: {
          top: u.top,
          left: u.left,
          minWidth: i
        },
        children: t.map((c) => c.divider ? /* @__PURE__ */ o.jsx(
          "hr",
          {
            className: "my-1 border-gray-200 dark:border-gray-700"
          },
          c.key
        ) : /* @__PURE__ */ o.jsxs(
          "button",
          {
            disabled: c.disabled,
            onClick: () => r(c),
            className: j(
              "w-full flex items-center gap-2 px-4 py-2 text-sm text-right",
              "transition-colors duration-150",
              c.disabled ? "text-gray-400 cursor-not-allowed" : c.danger ? "text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20" : "text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
            ),
            children: [
              c.icon && /* @__PURE__ */ o.jsx("span", { className: "flex-shrink-0 text-gray-400", children: c.icon }),
              /* @__PURE__ */ o.jsx("span", { children: c.label })
            ]
          },
          c.key
        ))
      }
    ),
    document.body
  ) : null;
}
function ef({
  trigger: e,
  items: t,
  onItemClick: r,
  placement: n = "bottom-end",
  minWidth: s = 160,
  disabled: a = !1,
  className: i
}) {
  const [l, u] = G(!1), d = _e(null), c = B(
    (m) => {
      m.stopPropagation(), a || u((p) => !p);
    },
    [a]
  ), f = B(
    (m) => {
      m.disabled || (r == null || r(m), u(!1));
    },
    [r]
  ), y = B(() => {
    u(!1);
  }, []);
  return /* @__PURE__ */ o.jsxs("div", { className: j("relative inline-block", i), children: [
    /* @__PURE__ */ o.jsx(
      "div",
      {
        ref: d,
        onClick: c,
        className: j(a && "opacity-50 cursor-not-allowed"),
        children: e
      }
    ),
    /* @__PURE__ */ o.jsx(
      Bi,
      {
        isOpen: l,
        items: t,
        onItemClick: f,
        onClose: y,
        triggerRef: d,
        placement: n,
        minWidth: s
      }
    )
  ] });
}
function tf({
  options: e,
  value: t,
  onChange: r,
  placeholder: n = "انتخاب کنید...",
  label: s,
  error: a,
  helperText: i,
  disabled: l = !1,
  clearable: u = !1,
  searchable: d = !1,
  fullWidth: c = !1,
  size: f = "md",
  className: y
}) {
  const [m, p] = G(!1), [b, g] = G(""), E = _e(null), N = _e(null), _ = _e(null), [w, S] = G({ top: 0, left: 0, width: 0 }), A = ke(
    () => e.find((R) => R.value === t),
    [e, t]
  ), D = ke(() => {
    if (!d || !b) return e;
    const R = b.toLowerCase();
    return e.filter(
      (V) => V.label.toLowerCase().includes(R)
    );
  }, [e, b, d]), U = ke(() => {
    const R = {}, V = [];
    return D.forEach((L) => {
      L.group ? (R[L.group] || (R[L.group] = []), R[L.group].push(L)) : V.push(L);
    }), { groups: R, ungrouped: V };
  }, [D]);
  me(() => {
    if (!m || !E.current) return;
    const R = E.current.getBoundingClientRect();
    S({
      top: R.bottom + 4,
      left: R.left,
      width: R.width
    });
  }, [m]), me(() => {
    if (!m) return;
    const R = (L) => {
      E.current && !E.current.contains(L.target) && _.current && !_.current.contains(L.target) && (p(!1), g(""));
    }, V = (L) => {
      L.key === "Escape" && (p(!1), g(""));
    };
    return document.addEventListener("mousedown", R), document.addEventListener("keydown", V), () => {
      document.removeEventListener("mousedown", R), document.removeEventListener("keydown", V);
    };
  }, [m]), me(() => {
    m && d && N.current && N.current.focus();
  }, [m, d]);
  const T = B(
    (R) => {
      R.disabled || (r == null || r(R.value), p(!1), g(""));
    },
    [r]
  ), M = B(
    (R) => {
      R.stopPropagation(), r == null || r(void 0);
    },
    [r]
  ), C = {
    sm: "px-3 py-1.5 text-sm",
    md: "px-4 py-2.5 text-sm",
    lg: "px-5 py-3 text-base"
  }, $ = (R) => /* @__PURE__ */ o.jsxs(
    "button",
    {
      onClick: () => T(R),
      disabled: R.disabled,
      className: j(
        "w-full flex items-center gap-2 px-4 py-2 text-right",
        "transition-colors",
        R.disabled ? "text-gray-400 cursor-not-allowed" : R.value === t ? "bg-purple-50 dark:bg-purple-900/30 text-purple-700 dark:text-purple-300" : "text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
      ),
      children: [
        R.icon && /* @__PURE__ */ o.jsx("span", { className: "flex-shrink-0", children: R.icon }),
        /* @__PURE__ */ o.jsx("span", { className: "flex-1", children: R.label }),
        R.value === t && /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4 text-purple-600", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M5 13l4 4L19 7" }) })
      ]
    },
    String(R.value)
  );
  return /* @__PURE__ */ o.jsxs("div", { className: j(c && "w-full", y), children: [
    s && /* @__PURE__ */ o.jsx("label", { className: "block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5", children: s }),
    /* @__PURE__ */ o.jsxs(
      "div",
      {
        ref: E,
        onClick: () => !l && p(!m),
        className: j(
          "relative flex items-center gap-2 cursor-pointer",
          "bg-white dark:bg-gray-800 border rounded-xl",
          "transition-all duration-200",
          C[f],
          a ? "border-red-500 focus-within:ring-2 focus-within:ring-red-500/20" : m ? "border-purple-500 ring-2 ring-purple-500/20" : "border-gray-300 dark:border-gray-600 hover:border-gray-400 dark:hover:border-gray-500",
          l && "opacity-60 cursor-not-allowed bg-gray-100 dark:bg-gray-900"
        ),
        children: [
          /* @__PURE__ */ o.jsxs("div", { className: "flex-1 flex items-center gap-2 min-w-0", children: [
            (A == null ? void 0 : A.icon) && /* @__PURE__ */ o.jsx("span", { className: "flex-shrink-0", children: A.icon }),
            /* @__PURE__ */ o.jsx(
              "span",
              {
                className: j(
                  "truncate",
                  A ? "text-gray-900 dark:text-white" : "text-gray-400"
                ),
                children: (A == null ? void 0 : A.label) || n
              }
            )
          ] }),
          u && t !== void 0 && !l && /* @__PURE__ */ o.jsx(
            "button",
            {
              onClick: M,
              className: "flex-shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300",
              children: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M6 18L18 6M6 6l12 12" }) })
            }
          ),
          /* @__PURE__ */ o.jsx(
            "svg",
            {
              className: j(
                "w-4 h-4 text-gray-400 transition-transform",
                m && "rotate-180"
              ),
              fill: "none",
              stroke: "currentColor",
              viewBox: "0 0 24 24",
              children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M19 9l-7 7-7-7" })
            }
          )
        ]
      }
    ),
    (i || a) && /* @__PURE__ */ o.jsx("p", { className: j(
      "text-xs mt-1.5",
      a ? "text-red-500" : "text-gray-500 dark:text-gray-400"
    ), children: a || i }),
    m && Nr(
      /* @__PURE__ */ o.jsxs(
        "div",
        {
          ref: _,
          className: j(
            "fixed z-50 py-1 max-h-60 overflow-auto",
            "bg-white dark:bg-gray-800",
            "border border-gray-200 dark:border-gray-700",
            "rounded-xl shadow-lg",
            "animate-in fade-in zoom-in-95 duration-150"
          ),
          style: {
            top: w.top,
            left: w.left,
            width: w.width
          },
          children: [
            d && /* @__PURE__ */ o.jsx("div", { className: "p-2 border-b border-gray-200 dark:border-gray-700", children: /* @__PURE__ */ o.jsx(
              "input",
              {
                ref: N,
                type: "text",
                value: b,
                onChange: (R) => g(R.target.value),
                placeholder: "جستجو...",
                className: j(
                  "w-full px-3 py-2 text-sm rounded-lg",
                  "bg-gray-100 dark:bg-gray-700",
                  "border-0 focus:ring-2 focus:ring-purple-500/20",
                  "text-gray-900 dark:text-white",
                  "placeholder-gray-400"
                )
              }
            ) }),
            D.length === 0 ? /* @__PURE__ */ o.jsx("div", { className: "px-4 py-3 text-sm text-gray-500 text-center", children: "نتیجه‌ای یافت نشد" }) : /* @__PURE__ */ o.jsxs(o.Fragment, { children: [
              U.ungrouped.map($),
              Object.entries(U.groups).map(([R, V]) => /* @__PURE__ */ o.jsxs("div", { children: [
                /* @__PURE__ */ o.jsx("div", { className: "px-4 py-2 text-xs font-medium text-gray-500 dark:text-gray-400 uppercase", children: R }),
                V.map($)
              ] }, R))
            ] })
          ]
        }
      ),
      document.body
    )
  ] });
}
function se(e) {
  return `Minified Redux error #${e}; visit https://redux.js.org/Errors?code=${e} for the full message or use the non-minified dev environment for full errors. `;
}
var Vi = typeof Symbol == "function" && Symbol.observable || "@@observable", ps = Vi, Zr = () => Math.random().toString(36).substring(7).split("").join("."), zi = {
  INIT: `@@redux/INIT${/* @__PURE__ */ Zr()}`,
  REPLACE: `@@redux/REPLACE${/* @__PURE__ */ Zr()}`,
  PROBE_UNKNOWN_ACTION: () => `@@redux/PROBE_UNKNOWN_ACTION${Zr()}`
}, rt = zi;
function Bt(e) {
  if (typeof e != "object" || e === null)
    return !1;
  let t = e;
  for (; Object.getPrototypeOf(t) !== null; )
    t = Object.getPrototypeOf(t);
  return Object.getPrototypeOf(e) === t || Object.getPrototypeOf(e) === null;
}
function Wi(e) {
  if (e === void 0)
    return "undefined";
  if (e === null)
    return "null";
  const t = typeof e;
  switch (t) {
    case "boolean":
    case "string":
    case "number":
    case "symbol":
    case "function":
      return t;
  }
  if (Array.isArray(e))
    return "array";
  if (Ki(e))
    return "date";
  if (qi(e))
    return "error";
  const r = Hi(e);
  switch (r) {
    case "Symbol":
    case "Promise":
    case "WeakMap":
    case "WeakSet":
    case "Map":
    case "Set":
      return r;
  }
  return Object.prototype.toString.call(e).slice(8, -1).toLowerCase().replace(/\s/g, "");
}
function Hi(e) {
  return typeof e.constructor == "function" ? e.constructor.name : null;
}
function qi(e) {
  return e instanceof Error || typeof e.message == "string" && e.constructor && typeof e.constructor.stackTraceLimit == "number";
}
function Ki(e) {
  return e instanceof Date ? !0 : typeof e.toDateString == "function" && typeof e.getDate == "function" && typeof e.setDate == "function";
}
function Be(e) {
  let t = typeof e;
  return process.env.NODE_ENV !== "production" && (t = Wi(e)), t;
}
function vo(e, t, r) {
  if (typeof e != "function")
    throw new Error(process.env.NODE_ENV === "production" ? se(2) : `Expected the root reducer to be a function. Instead, received: '${Be(e)}'`);
  if (typeof t == "function" && typeof r == "function" || typeof r == "function" && typeof arguments[3] == "function")
    throw new Error(process.env.NODE_ENV === "production" ? se(0) : "It looks like you are passing several store enhancers to createStore(). This is not supported. Instead, compose them together to a single function. See https://redux.js.org/tutorials/fundamentals/part-4-store#creating-a-store-with-enhancers for an example.");
  if (typeof t == "function" && typeof r > "u" && (r = t, t = void 0), typeof r < "u") {
    if (typeof r != "function")
      throw new Error(process.env.NODE_ENV === "production" ? se(1) : `Expected the enhancer to be a function. Instead, received: '${Be(r)}'`);
    return r(vo)(e, t);
  }
  let n = e, s = t, a = /* @__PURE__ */ new Map(), i = a, l = 0, u = !1;
  function d() {
    i === a && (i = /* @__PURE__ */ new Map(), a.forEach((g, E) => {
      i.set(E, g);
    }));
  }
  function c() {
    if (u)
      throw new Error(process.env.NODE_ENV === "production" ? se(3) : "You may not call store.getState() while the reducer is executing. The reducer has already received the state as an argument. Pass it down from the top reducer instead of reading it from the store.");
    return s;
  }
  function f(g) {
    if (typeof g != "function")
      throw new Error(process.env.NODE_ENV === "production" ? se(4) : `Expected the listener to be a function. Instead, received: '${Be(g)}'`);
    if (u)
      throw new Error(process.env.NODE_ENV === "production" ? se(5) : "You may not call store.subscribe() while the reducer is executing. If you would like to be notified after the store has been updated, subscribe from a component and invoke store.getState() in the callback to access the latest state. See https://redux.js.org/api/store#subscribelistener for more details.");
    let E = !0;
    d();
    const N = l++;
    return i.set(N, g), function() {
      if (E) {
        if (u)
          throw new Error(process.env.NODE_ENV === "production" ? se(6) : "You may not unsubscribe from a store listener while the reducer is executing. See https://redux.js.org/api/store#subscribelistener for more details.");
        E = !1, d(), i.delete(N), a = null;
      }
    };
  }
  function y(g) {
    if (!Bt(g))
      throw new Error(process.env.NODE_ENV === "production" ? se(7) : `Actions must be plain objects. Instead, the actual type was: '${Be(g)}'. You may need to add middleware to your store setup to handle dispatching other values, such as 'redux-thunk' to handle dispatching functions. See https://redux.js.org/tutorials/fundamentals/part-4-store#middleware and https://redux.js.org/tutorials/fundamentals/part-6-async-logic#using-the-redux-thunk-middleware for examples.`);
    if (typeof g.type > "u")
      throw new Error(process.env.NODE_ENV === "production" ? se(8) : 'Actions may not have an undefined "type" property. You may have misspelled an action type string constant.');
    if (typeof g.type != "string")
      throw new Error(process.env.NODE_ENV === "production" ? se(17) : `Action "type" property must be a string. Instead, the actual type was: '${Be(g.type)}'. Value was: '${g.type}' (stringified)`);
    if (u)
      throw new Error(process.env.NODE_ENV === "production" ? se(9) : "Reducers may not dispatch actions.");
    try {
      u = !0, s = n(s, g);
    } finally {
      u = !1;
    }
    return (a = i).forEach((N) => {
      N();
    }), g;
  }
  function m(g) {
    if (typeof g != "function")
      throw new Error(process.env.NODE_ENV === "production" ? se(10) : `Expected the nextReducer to be a function. Instead, received: '${Be(g)}`);
    n = g, y({
      type: rt.REPLACE
    });
  }
  function p() {
    const g = f;
    return {
      /**
       * The minimal observable subscription method.
       * @param observer Any object that can be used as an observer.
       * The observer object should have a `next` method.
       * @returns An object with an `unsubscribe` method that can
       * be used to unsubscribe the observable from the store, and prevent further
       * emission of values from the observable.
       */
      subscribe(E) {
        if (typeof E != "object" || E === null)
          throw new Error(process.env.NODE_ENV === "production" ? se(11) : `Expected the observer to be an object. Instead, received: '${Be(E)}'`);
        function N() {
          const w = E;
          w.next && w.next(c());
        }
        return N(), {
          unsubscribe: g(N)
        };
      },
      [ps]() {
        return this;
      }
    };
  }
  return y({
    type: rt.INIT
  }), {
    dispatch: y,
    subscribe: f,
    getState: c,
    replaceReducer: m,
    [ps]: p
  };
}
function ms(e) {
  typeof console < "u" && typeof console.error == "function" && console.error(e);
  try {
    throw new Error(e);
  } catch {
  }
}
function Ji(e, t, r, n) {
  const s = Object.keys(t), a = r && r.type === rt.INIT ? "preloadedState argument passed to createStore" : "previous state received by the reducer";
  if (s.length === 0)
    return "Store does not have a valid reducer. Make sure the argument passed to combineReducers is an object whose values are reducers.";
  if (!Bt(e))
    return `The ${a} has unexpected type of "${Be(e)}". Expected argument to be an object with the following keys: "${s.join('", "')}"`;
  const i = Object.keys(e).filter((l) => !t.hasOwnProperty(l) && !n[l]);
  if (i.forEach((l) => {
    n[l] = !0;
  }), !(r && r.type === rt.REPLACE) && i.length > 0)
    return `Unexpected ${i.length > 1 ? "keys" : "key"} "${i.join('", "')}" found in ${a}. Expected to find one of the known reducer keys instead: "${s.join('", "')}". Unexpected keys will be ignored.`;
}
function Qi(e) {
  Object.keys(e).forEach((t) => {
    const r = e[t];
    if (typeof r(void 0, {
      type: rt.INIT
    }) > "u")
      throw new Error(process.env.NODE_ENV === "production" ? se(12) : `The slice reducer for key "${t}" returned undefined during initialization. If the state passed to the reducer is undefined, you must explicitly return the initial state. The initial state may not be undefined. If you don't want to set a value for this reducer, you can use null instead of undefined.`);
    if (typeof r(void 0, {
      type: rt.PROBE_UNKNOWN_ACTION()
    }) > "u")
      throw new Error(process.env.NODE_ENV === "production" ? se(13) : `The slice reducer for key "${t}" returned undefined when probed with a random type. Don't try to handle '${rt.INIT}' or other actions in "redux/*" namespace. They are considered private. Instead, you must return the current state for any unknown actions, unless it is undefined, in which case you must return the initial state, regardless of the action type. The initial state may not be undefined, but can be null.`);
  });
}
function wo(e) {
  const t = Object.keys(e), r = {};
  for (let i = 0; i < t.length; i++) {
    const l = t[i];
    process.env.NODE_ENV !== "production" && typeof e[l] > "u" && ms(`No reducer provided for key "${l}"`), typeof e[l] == "function" && (r[l] = e[l]);
  }
  const n = Object.keys(r);
  let s;
  process.env.NODE_ENV !== "production" && (s = {});
  let a;
  try {
    Qi(r);
  } catch (i) {
    a = i;
  }
  return function(l = {}, u) {
    if (a)
      throw a;
    if (process.env.NODE_ENV !== "production") {
      const f = Ji(l, r, u, s);
      f && ms(f);
    }
    let d = !1;
    const c = {};
    for (let f = 0; f < n.length; f++) {
      const y = n[f], m = r[y], p = l[y], b = m(p, u);
      if (typeof b > "u") {
        const g = u && u.type;
        throw new Error(process.env.NODE_ENV === "production" ? se(14) : `When called with an action of type ${g ? `"${String(g)}"` : "(unknown type)"}, the slice reducer for key "${y}" returned undefined. To ignore an action, you must explicitly return the previous state. If you want this reducer to hold no value, you can return null instead of undefined.`);
      }
      c[y] = b, d = d || b !== p;
    }
    return d = d || n.length !== Object.keys(l).length, d ? c : l;
  };
}
function hr(...e) {
  return e.length === 0 ? (t) => t : e.length === 1 ? e[0] : e.reduce((t, r) => (...n) => t(r(...n)));
}
function Gi(...e) {
  return (t) => (r, n) => {
    const s = t(r, n);
    let a = () => {
      throw new Error(process.env.NODE_ENV === "production" ? se(15) : "Dispatching while constructing your middleware is not allowed. Other middleware would not be applied to this dispatch.");
    };
    const i = {
      getState: s.getState,
      dispatch: (u, ...d) => a(u, ...d)
    }, l = e.map((u) => u(i));
    return a = hr(...l)(s.dispatch), {
      ...s,
      dispatch: a
    };
  };
}
function Eo(e) {
  return Bt(e) && "type" in e && typeof e.type == "string";
}
var ko = Symbol.for("immer-nothing"), gs = Symbol.for("immer-draftable"), de = Symbol.for("immer-state"), Yi = process.env.NODE_ENV !== "production" ? [
  // All error codes, starting by 0:
  function(e) {
    return `The plugin for '${e}' has not been loaded into Immer. To enable the plugin, import and call \`enable${e}()\` when initializing your application.`;
  },
  function(e) {
    return `produce can only be called on things that are draftable: plain objects, arrays, Map, Set or classes that are marked with '[immerable]: true'. Got '${e}'`;
  },
  "This object has been frozen and should not be mutated",
  function(e) {
    return "Cannot use a proxy that has been revoked. Did you pass an object from inside an immer function to an async process? " + e;
  },
  "An immer producer returned a new value *and* modified its draft. Either return a new value *or* modify the draft.",
  "Immer forbids circular references",
  "The first or second argument to `produce` must be a function",
  "The third argument to `produce` must be a function or undefined",
  "First argument to `createDraft` must be a plain object, an array, or an immerable object",
  "First argument to `finishDraft` must be a draft returned by `createDraft`",
  function(e) {
    return `'current' expects a draft, got: ${e}`;
  },
  "Object.defineProperty() cannot be used on an Immer draft",
  "Object.setPrototypeOf() cannot be used on an Immer draft",
  "Immer only supports deleting array indices",
  "Immer only supports setting array indices and the 'length' property",
  function(e) {
    return `'original' expects a draft, got: ${e}`;
  }
  // Note: if more errors are added, the errorOffset in Patches.ts should be increased
  // See Patches.ts for additional errors
] : [];
function be(e, ...t) {
  if (process.env.NODE_ENV !== "production") {
    const r = Yi[e], n = Je(r) ? r.apply(null, t) : r;
    throw new Error(`[Immer] ${n}`);
  }
  throw new Error(
    `[Immer] minified error nr: ${e}. Full error at: https://bit.ly/3cXEKWf`
  );
}
var ve = Object, yt = ve.getPrototypeOf, pr = "constructor", jr = "prototype", fn = "configurable", mr = "enumerable", or = "writable", Pt = "value", Ue = (e) => !!e && !!e[de];
function Re(e) {
  var t;
  return e ? No(e) || _r(e) || !!e[gs] || !!((t = e[pr]) != null && t[gs]) || Rr(e) || Or(e) : !1;
}
var Xi = ve[jr][pr].toString(), ys = /* @__PURE__ */ new WeakMap();
function No(e) {
  if (!e || !Un(e))
    return !1;
  const t = yt(e);
  if (t === null || t === ve[jr])
    return !0;
  const r = ve.hasOwnProperty.call(t, pr) && t[pr];
  if (r === Object)
    return !0;
  if (!Je(r))
    return !1;
  let n = ys.get(r);
  return n === void 0 && (n = Function.toString.call(r), ys.set(r, n)), n === Xi;
}
function Sr(e, t, r = !0) {
  Vt(e) === 0 ? (r ? Reflect.ownKeys(e) : ve.keys(e)).forEach((s) => {
    t(s, e[s], e);
  }) : e.forEach((n, s) => t(s, n, e));
}
function Vt(e) {
  const t = e[de];
  return t ? t.type_ : _r(e) ? 1 : Rr(e) ? 2 : Or(e) ? 3 : 0;
}
var xs = (e, t, r = Vt(e)) => r === 2 ? e.has(t) : ve[jr].hasOwnProperty.call(e, t), hn = (e, t, r = Vt(e)) => (
  // @ts-ignore
  r === 2 ? e.get(t) : e[t]
), gr = (e, t, r, n = Vt(e)) => {
  n === 2 ? e.set(t, r) : n === 3 ? e.add(r) : e[t] = r;
};
function Zi(e, t) {
  return e === t ? e !== 0 || 1 / e === 1 / t : e !== e && t !== t;
}
var _r = Array.isArray, Rr = (e) => e instanceof Map, Or = (e) => e instanceof Set, Un = (e) => typeof e == "object", Je = (e) => typeof e == "function", en = (e) => typeof e == "boolean";
function el(e) {
  const t = +e;
  return Number.isInteger(t) && String(t) === e;
}
var Me = (e) => e.copy_ || e.base_, $n = (e) => e.modified_ ? e.copy_ : e.base_;
function pn(e, t) {
  if (Rr(e))
    return new Map(e);
  if (Or(e))
    return new Set(e);
  if (_r(e))
    return Array[jr].slice.call(e);
  const r = No(e);
  if (t === !0 || t === "class_only" && !r) {
    const n = ve.getOwnPropertyDescriptors(e);
    delete n[de];
    let s = Reflect.ownKeys(n);
    for (let a = 0; a < s.length; a++) {
      const i = s[a], l = n[i];
      l[or] === !1 && (l[or] = !0, l[fn] = !0), (l.get || l.set) && (n[i] = {
        [fn]: !0,
        [or]: !0,
        // could live with !!desc.set as well here...
        [mr]: l[mr],
        [Pt]: e[i]
      });
    }
    return ve.create(yt(e), n);
  } else {
    const n = yt(e);
    if (n !== null && r)
      return { ...e };
    const s = ve.create(n);
    return ve.assign(s, e);
  }
}
function Bn(e, t = !1) {
  return Tr(e) || Ue(e) || !Re(e) || (Vt(e) > 1 && ve.defineProperties(e, {
    set: Zt,
    add: Zt,
    clear: Zt,
    delete: Zt
  }), ve.freeze(e), t && Sr(
    e,
    (r, n) => {
      Bn(n, !0);
    },
    !1
  )), e;
}
function tl() {
  be(2);
}
var Zt = {
  [Pt]: tl
};
function Tr(e) {
  return e === null || !Un(e) ? !0 : ve.isFrozen(e);
}
var yr = "MapSet", mn = "Patches", bs = "ArrayMethods", jo = {};
function st(e) {
  const t = jo[e];
  return t || be(0, e), t;
}
var vs = (e) => !!jo[e], Lt, So = () => Lt, rl = (e, t) => ({
  drafts_: [],
  parent_: e,
  immer_: t,
  // Whenever the modified draft contains a draft from another scope, we
  // need to prevent auto-freezing so the unowned draft can be finalized.
  canAutoFreeze_: !0,
  unfinalizedDrafts_: 0,
  handledSet_: /* @__PURE__ */ new Set(),
  processedForPatches_: /* @__PURE__ */ new Set(),
  mapSetPlugin_: vs(yr) ? st(yr) : void 0,
  arrayMethodsPlugin_: vs(bs) ? st(bs) : void 0
});
function ws(e, t) {
  t && (e.patchPlugin_ = st(mn), e.patches_ = [], e.inversePatches_ = [], e.patchListener_ = t);
}
function gn(e) {
  yn(e), e.drafts_.forEach(nl), e.drafts_ = null;
}
function yn(e) {
  e === Lt && (Lt = e.parent_);
}
var Es = (e) => Lt = rl(Lt, e);
function nl(e) {
  const t = e[de];
  t.type_ === 0 || t.type_ === 1 ? t.revoke_() : t.revoked_ = !0;
}
function ks(e, t) {
  t.unfinalizedDrafts_ = t.drafts_.length;
  const r = t.drafts_[0];
  if (e !== void 0 && e !== r) {
    r[de].modified_ && (gn(t), be(4)), Re(e) && (e = Ns(t, e));
    const { patchPlugin_: s } = t;
    s && s.generateReplacementPatches_(
      r[de].base_,
      e,
      t
    );
  } else
    e = Ns(t, r);
  return sl(t, e, !0), gn(t), t.patches_ && t.patchListener_(t.patches_, t.inversePatches_), e !== ko ? e : void 0;
}
function Ns(e, t) {
  if (Tr(t))
    return t;
  const r = t[de];
  if (!r)
    return xr(t, e.handledSet_, e);
  if (!Cr(r, e))
    return t;
  if (!r.modified_)
    return r.base_;
  if (!r.finalized_) {
    const { callbacks_: n } = r;
    if (n)
      for (; n.length > 0; )
        n.pop()(e);
    Oo(r, e);
  }
  return r.copy_;
}
function sl(e, t, r = !1) {
  !e.parent_ && e.immer_.autoFreeze_ && e.canAutoFreeze_ && Bn(t, r);
}
function _o(e) {
  e.finalized_ = !0, e.scope_.unfinalizedDrafts_--;
}
var Cr = (e, t) => e.scope_ === t, ol = [];
function Ro(e, t, r, n) {
  const s = Me(e), a = e.type_;
  if (n !== void 0 && hn(s, n, a) === t) {
    gr(s, n, r, a);
    return;
  }
  if (!e.draftLocations_) {
    const l = e.draftLocations_ = /* @__PURE__ */ new Map();
    Sr(s, (u, d) => {
      if (Ue(d)) {
        const c = l.get(d) || [];
        c.push(u), l.set(d, c);
      }
    });
  }
  const i = e.draftLocations_.get(t) ?? ol;
  for (const l of i)
    gr(s, l, r, a);
}
function al(e, t, r) {
  e.callbacks_.push(function(s) {
    var l;
    const a = t;
    if (!a || !Cr(a, s))
      return;
    (l = s.mapSetPlugin_) == null || l.fixSetContents(a);
    const i = $n(a);
    Ro(e, a.draft_ ?? a, i, r), Oo(a, s);
  });
}
function Oo(e, t) {
  var n;
  if (e.modified_ && !e.finalized_ && (e.type_ === 3 || e.type_ === 1 && e.allIndicesReassigned_ || (((n = e.assigned_) == null ? void 0 : n.size) ?? 0) > 0)) {
    const { patchPlugin_: s } = t;
    if (s) {
      const a = s.getPath(e);
      a && s.generatePatches_(e, a, t);
    }
    _o(e);
  }
}
function il(e, t, r) {
  const { scope_: n } = e;
  if (Ue(r)) {
    const s = r[de];
    Cr(s, n) && s.callbacks_.push(function() {
      ar(e);
      const i = $n(s);
      Ro(e, r, i, t);
    });
  } else Re(r) && e.callbacks_.push(function() {
    const a = Me(e);
    e.type_ === 3 ? a.has(r) && xr(r, n.handledSet_, n) : hn(a, t, e.type_) === r && n.drafts_.length > 1 && (e.assigned_.get(t) ?? !1) === !0 && e.copy_ && xr(
      hn(e.copy_, t, e.type_),
      n.handledSet_,
      n
    );
  });
}
function xr(e, t, r) {
  return !r.immer_.autoFreeze_ && r.unfinalizedDrafts_ < 1 || Ue(e) || t.has(e) || !Re(e) || Tr(e) || (t.add(e), Sr(e, (n, s) => {
    if (Ue(s)) {
      const a = s[de];
      if (Cr(a, r)) {
        const i = $n(a);
        gr(e, n, i, e.type_), _o(a);
      }
    } else Re(s) && xr(s, t, r);
  })), e;
}
function ll(e, t) {
  const r = _r(e), n = {
    type_: r ? 1 : 0,
    // Track which produce call this is associated with.
    scope_: t ? t.scope_ : So(),
    // True for both shallow and deep changes.
    modified_: !1,
    // Used during finalization.
    finalized_: !1,
    // Track which properties have been assigned (true) or deleted (false).
    // actually instantiated in `prepareCopy()`
    assigned_: void 0,
    // The parent draft state.
    parent_: t,
    // The base state.
    base_: e,
    // The base proxy.
    draft_: null,
    // set below
    // The base copy with any updated values.
    copy_: null,
    // Called by the `produce` function.
    revoke_: null,
    isManual_: !1,
    // `callbacks` actually gets assigned in `createProxy`
    callbacks_: void 0
  };
  let s = n, a = br;
  r && (s = [n], a = It);
  const { revoke: i, proxy: l } = Proxy.revocable(s, a);
  return n.draft_ = l, n.revoke_ = i, [l, n];
}
var br = {
  get(e, t) {
    if (t === de)
      return e;
    let r = e.scope_.arrayMethodsPlugin_;
    const n = e.type_ === 1 && typeof t == "string";
    if (n && r != null && r.isArrayOperationMethod(t))
      return r.createMethodInterceptor(e, t);
    const s = Me(e);
    if (!xs(s, t, e.type_))
      return cl(e, s, t);
    const a = s[t];
    if (e.finalized_ || !Re(a) || n && e.operationMethod && (r != null && r.isMutatingArrayMethod(
      e.operationMethod
    )) && el(t))
      return a;
    if (a === tn(e.base_, t)) {
      ar(e);
      const i = e.type_ === 1 ? +t : t, l = bn(e.scope_, a, e, i);
      return e.copy_[i] = l;
    }
    return a;
  },
  has(e, t) {
    return t in Me(e);
  },
  ownKeys(e) {
    return Reflect.ownKeys(Me(e));
  },
  set(e, t, r) {
    const n = To(Me(e), t);
    if (n != null && n.set)
      return n.set.call(e.draft_, r), !0;
    if (!e.modified_) {
      const s = tn(Me(e), t), a = s == null ? void 0 : s[de];
      if (a && a.base_ === r)
        return e.copy_[t] = r, e.assigned_.set(t, !1), !0;
      if (Zi(r, s) && (r !== void 0 || xs(e.base_, t, e.type_)))
        return !0;
      ar(e), xn(e);
    }
    return e.copy_[t] === r && // special case: handle new props with value 'undefined'
    (r !== void 0 || t in e.copy_) || // special case: NaN
    Number.isNaN(r) && Number.isNaN(e.copy_[t]) || (e.copy_[t] = r, e.assigned_.set(t, !0), il(e, t, r)), !0;
  },
  deleteProperty(e, t) {
    return ar(e), tn(e.base_, t) !== void 0 || t in e.base_ ? (e.assigned_.set(t, !1), xn(e)) : e.assigned_.delete(t), e.copy_ && delete e.copy_[t], !0;
  },
  // Note: We never coerce `desc.value` into an Immer draft, because we can't make
  // the same guarantee in ES5 mode.
  getOwnPropertyDescriptor(e, t) {
    const r = Me(e), n = Reflect.getOwnPropertyDescriptor(r, t);
    return n && {
      [or]: !0,
      [fn]: e.type_ !== 1 || t !== "length",
      [mr]: n[mr],
      [Pt]: r[t]
    };
  },
  defineProperty() {
    be(11);
  },
  getPrototypeOf(e) {
    return yt(e.base_);
  },
  setPrototypeOf() {
    be(12);
  }
}, It = {};
for (let e in br) {
  let t = br[e];
  It[e] = function() {
    const r = arguments;
    return r[0] = r[0][0], t.apply(this, r);
  };
}
It.deleteProperty = function(e, t) {
  return process.env.NODE_ENV !== "production" && isNaN(parseInt(t)) && be(13), It.set.call(this, e, t, void 0);
};
It.set = function(e, t, r) {
  return process.env.NODE_ENV !== "production" && t !== "length" && isNaN(parseInt(t)) && be(14), br.set.call(this, e[0], t, r, e[0]);
};
function tn(e, t) {
  const r = e[de];
  return (r ? Me(r) : e)[t];
}
function cl(e, t, r) {
  var s;
  const n = To(t, r);
  return n ? Pt in n ? n[Pt] : (
    // This is a very special case, if the prop is a getter defined by the
    // prototype, we should invoke it with the draft as context!
    (s = n.get) == null ? void 0 : s.call(e.draft_)
  ) : void 0;
}
function To(e, t) {
  if (!(t in e))
    return;
  let r = yt(e);
  for (; r; ) {
    const n = Object.getOwnPropertyDescriptor(r, t);
    if (n)
      return n;
    r = yt(r);
  }
}
function xn(e) {
  e.modified_ || (e.modified_ = !0, e.parent_ && xn(e.parent_));
}
function ar(e) {
  e.copy_ || (e.assigned_ = /* @__PURE__ */ new Map(), e.copy_ = pn(
    e.base_,
    e.scope_.immer_.useStrictShallowCopy_
  ));
}
var dl = class {
  constructor(e) {
    this.autoFreeze_ = !0, this.useStrictShallowCopy_ = !1, this.useStrictIteration_ = !1, this.produce = (t, r, n) => {
      if (Je(t) && !Je(r)) {
        const a = r;
        r = t;
        const i = this;
        return function(u = a, ...d) {
          return i.produce(u, (c) => r.call(this, c, ...d));
        };
      }
      Je(r) || be(6), n !== void 0 && !Je(n) && be(7);
      let s;
      if (Re(t)) {
        const a = Es(this), i = bn(a, t, void 0);
        let l = !0;
        try {
          s = r(i), l = !1;
        } finally {
          l ? gn(a) : yn(a);
        }
        return ws(a, n), ks(s, a);
      } else if (!t || !Un(t)) {
        if (s = r(t), s === void 0 && (s = t), s === ko && (s = void 0), this.autoFreeze_ && Bn(s, !0), n) {
          const a = [], i = [];
          st(mn).generateReplacementPatches_(t, s, {
            patches_: a,
            inversePatches_: i
          }), n(a, i);
        }
        return s;
      } else
        be(1, t);
    }, this.produceWithPatches = (t, r) => {
      if (Je(t))
        return (i, ...l) => this.produceWithPatches(i, (u) => t(u, ...l));
      let n, s;
      return [this.produce(t, r, (i, l) => {
        n = i, s = l;
      }), n, s];
    }, en(e == null ? void 0 : e.autoFreeze) && this.setAutoFreeze(e.autoFreeze), en(e == null ? void 0 : e.useStrictShallowCopy) && this.setUseStrictShallowCopy(e.useStrictShallowCopy), en(e == null ? void 0 : e.useStrictIteration) && this.setUseStrictIteration(e.useStrictIteration);
  }
  createDraft(e) {
    Re(e) || be(8), Ue(e) && (e = ul(e));
    const t = Es(this), r = bn(t, e, void 0);
    return r[de].isManual_ = !0, yn(t), r;
  }
  finishDraft(e, t) {
    const r = e && e[de];
    (!r || !r.isManual_) && be(9);
    const { scope_: n } = r;
    return ws(n, t), ks(void 0, n);
  }
  /**
   * Pass true to automatically freeze all copies created by Immer.
   *
   * By default, auto-freezing is enabled.
   */
  setAutoFreeze(e) {
    this.autoFreeze_ = e;
  }
  /**
   * Pass true to enable strict shallow copy.
   *
   * By default, immer does not copy the object descriptors such as getter, setter and non-enumrable properties.
   */
  setUseStrictShallowCopy(e) {
    this.useStrictShallowCopy_ = e;
  }
  /**
   * Pass false to use faster iteration that skips non-enumerable properties
   * but still handles symbols for compatibility.
   *
   * By default, strict iteration is enabled (includes all own properties).
   */
  setUseStrictIteration(e) {
    this.useStrictIteration_ = e;
  }
  shouldUseStrictIteration() {
    return this.useStrictIteration_;
  }
  applyPatches(e, t) {
    let r;
    for (r = t.length - 1; r >= 0; r--) {
      const s = t[r];
      if (s.path.length === 0 && s.op === "replace") {
        e = s.value;
        break;
      }
    }
    r > -1 && (t = t.slice(r + 1));
    const n = st(mn).applyPatches_;
    return Ue(e) ? n(e, t) : this.produce(
      e,
      (s) => n(s, t)
    );
  }
};
function bn(e, t, r, n) {
  const [s, a] = Rr(t) ? st(yr).proxyMap_(t, r) : Or(t) ? st(yr).proxySet_(t, r) : ll(t, r);
  return ((r == null ? void 0 : r.scope_) ?? So()).drafts_.push(s), a.callbacks_ = (r == null ? void 0 : r.callbacks_) ?? [], a.key_ = n, r && n !== void 0 ? al(r, a, n) : a.callbacks_.push(function(u) {
    var c;
    (c = u.mapSetPlugin_) == null || c.fixSetContents(a);
    const { patchPlugin_: d } = u;
    a.modified_ && d && d.generatePatches_(a, [], u);
  }), s;
}
function ul(e) {
  return Ue(e) || be(10, e), Co(e);
}
function Co(e) {
  if (!Re(e) || Tr(e))
    return e;
  const t = e[de];
  let r, n = !0;
  if (t) {
    if (!t.modified_)
      return t.base_;
    t.finalized_ = !0, r = pn(e, t.scope_.immer_.useStrictShallowCopy_), n = t.scope_.immer_.shouldUseStrictIteration();
  } else
    r = pn(e, !0);
  return Sr(
    r,
    (s, a) => {
      gr(r, s, Co(a));
    },
    n
  ), t && (t.finalized_ = !1), r;
}
var fl = new dl(), Ao = fl.produce;
function Do(e) {
  return ({ dispatch: r, getState: n }) => (s) => (a) => typeof a == "function" ? a(r, n, e) : s(a);
}
var hl = Do(), pl = Do, ml = typeof window < "u" && window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ ? window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ : function() {
  if (arguments.length !== 0)
    return typeof arguments[0] == "object" ? hr : hr.apply(null, arguments);
}, Mo = (e) => e && typeof e.match == "function";
function At(e, t) {
  function r(...n) {
    if (t) {
      let s = t(...n);
      if (!s)
        throw new Error(process.env.NODE_ENV === "production" ? Z(0) : "prepareAction did not return an object");
      return {
        type: e,
        payload: s.payload,
        ..."meta" in s && {
          meta: s.meta
        },
        ..."error" in s && {
          error: s.error
        }
      };
    }
    return {
      type: e,
      payload: n[0]
    };
  }
  return r.toString = () => `${e}`, r.type = e, r.match = (n) => Eo(n) && n.type === e, r;
}
function gl(e) {
  return typeof e == "function" && "type" in e && // hasMatchFunction only wants Matchers but I don't see the point in rewriting it
  Mo(e);
}
function yl(e) {
  const t = e ? `${e}`.split("/") : [], r = t[t.length - 1] || "actionCreator";
  return `Detected an action creator with type "${e || "unknown"}" being dispatched. 
Make sure you're calling the action creator before dispatching, i.e. \`dispatch(${r}())\` instead of \`dispatch(${r})\`. This is necessary even if the action has no payload.`;
}
function xl(e = {}) {
  if (process.env.NODE_ENV === "production")
    return () => (r) => (n) => r(n);
  const {
    isActionCreator: t = gl
  } = e;
  return () => (r) => (n) => (t(n) && console.warn(yl(n.type)), r(n));
}
function Po(e, t) {
  let r = 0;
  return {
    measureTime(n) {
      const s = Date.now();
      try {
        return n();
      } finally {
        const a = Date.now();
        r += a - s;
      }
    },
    warnIfExceeded() {
      r > e && console.warn(`${t} took ${r}ms, which is more than the warning threshold of ${e}ms. 
If your state or actions are very large, you may want to disable the middleware as it might cause too much of a slowdown in development mode. See https://redux-toolkit.js.org/api/getDefaultMiddleware for instructions.
It is disabled in production builds, so you don't need to worry about that.`);
    }
  };
}
var Lo = class Ot extends Array {
  constructor(...t) {
    super(...t), Object.setPrototypeOf(this, Ot.prototype);
  }
  static get [Symbol.species]() {
    return Ot;
  }
  concat(...t) {
    return super.concat.apply(this, t);
  }
  prepend(...t) {
    return t.length === 1 && Array.isArray(t[0]) ? new Ot(...t[0].concat(this)) : new Ot(...t.concat(this));
  }
};
function js(e) {
  return Re(e) ? Ao(e, () => {
  }) : e;
}
function er(e, t, r) {
  return e.has(t) ? e.get(t) : e.set(t, r(t)).get(t);
}
function bl(e) {
  return typeof e != "object" || e == null || Object.isFrozen(e);
}
function vl(e, t, r) {
  const n = Io(e, t, r);
  return {
    detectMutations() {
      return Fo(e, t, n, r);
    }
  };
}
function Io(e, t = [], r, n = "", s = /* @__PURE__ */ new Set()) {
  const a = {
    value: r
  };
  if (!e(r) && !s.has(r)) {
    s.add(r), a.children = {};
    const i = t.length > 0;
    for (const l in r) {
      const u = n ? n + "." + l : l;
      i && t.some((c) => c instanceof RegExp ? c.test(u) : u === c) || (a.children[l] = Io(e, t, r[l], u));
    }
  }
  return a;
}
function Fo(e, t = [], r, n, s = !1, a = "") {
  const i = r ? r.value : void 0, l = i === n;
  if (s && !l && !Number.isNaN(n))
    return {
      wasMutated: !0,
      path: a
    };
  if (e(i) || e(n))
    return {
      wasMutated: !1
    };
  const u = {};
  for (let c in r.children)
    u[c] = !0;
  for (let c in n)
    u[c] = !0;
  const d = t.length > 0;
  for (let c in u) {
    const f = a ? a + "." + c : c;
    if (d && t.some((p) => p instanceof RegExp ? p.test(f) : f === p))
      continue;
    const y = Fo(e, t, r.children[c], n[c], l, f);
    if (y.wasMutated)
      return y;
  }
  return {
    wasMutated: !1
  };
}
function wl(e = {}) {
  if (process.env.NODE_ENV === "production")
    return () => (t) => (r) => t(r);
  {
    let t = function(l, u, d, c) {
      return JSON.stringify(l, r(u, c), d);
    }, r = function(l, u) {
      let d = [], c = [];
      return u || (u = function(f, y) {
        return d[0] === y ? "[Circular ~]" : "[Circular ~." + c.slice(0, d.indexOf(y)).join(".") + "]";
      }), function(f, y) {
        if (d.length > 0) {
          var m = d.indexOf(this);
          ~m ? d.splice(m + 1) : d.push(this), ~m ? c.splice(m, 1 / 0, f) : c.push(f), ~d.indexOf(y) && (y = u.call(this, f, y));
        } else d.push(y);
        return l == null ? y : l.call(this, f, y);
      };
    }, {
      isImmutable: n = bl,
      ignoredPaths: s,
      warnAfter: a = 32
    } = e;
    const i = vl.bind(null, n, s);
    return ({
      getState: l
    }) => {
      let u = l(), d = i(u), c;
      return (f) => (y) => {
        const m = Po(a, "ImmutableStateInvariantMiddleware");
        m.measureTime(() => {
          if (u = l(), c = d.detectMutations(), d = i(u), c.wasMutated)
            throw new Error(process.env.NODE_ENV === "production" ? Z(19) : `A state mutation was detected between dispatches, in the path '${c.path || ""}'.  This may cause incorrect behavior. (https://redux.js.org/style-guide/style-guide#do-not-mutate-state)`);
        });
        const p = f(y);
        return m.measureTime(() => {
          if (u = l(), c = d.detectMutations(), d = i(u), c.wasMutated)
            throw new Error(process.env.NODE_ENV === "production" ? Z(20) : `A state mutation was detected inside a dispatch, in the path: ${c.path || ""}. Take a look at the reducer(s) handling the action ${t(y)}. (https://redux.js.org/style-guide/style-guide#do-not-mutate-state)`);
        }), m.warnIfExceeded(), p;
      };
    };
  }
}
function Uo(e) {
  const t = typeof e;
  return e == null || t === "string" || t === "boolean" || t === "number" || Array.isArray(e) || Bt(e);
}
function vn(e, t = "", r = Uo, n, s = [], a) {
  let i;
  if (!r(e))
    return {
      keyPath: t || "<root>",
      value: e
    };
  if (typeof e != "object" || e === null || a != null && a.has(e)) return !1;
  const l = n != null ? n(e) : Object.entries(e), u = s.length > 0;
  for (const [d, c] of l) {
    const f = t ? t + "." + d : d;
    if (!(u && s.some((m) => m instanceof RegExp ? m.test(f) : f === m))) {
      if (!r(c))
        return {
          keyPath: f,
          value: c
        };
      if (typeof c == "object" && (i = vn(c, f, r, n, s, a), i))
        return i;
    }
  }
  return a && $o(e) && a.add(e), !1;
}
function $o(e) {
  if (!Object.isFrozen(e)) return !1;
  for (const t of Object.values(e))
    if (!(typeof t != "object" || t === null) && !$o(t))
      return !1;
  return !0;
}
function El(e = {}) {
  if (process.env.NODE_ENV === "production")
    return () => (t) => (r) => t(r);
  {
    const {
      isSerializable: t = Uo,
      getEntries: r,
      ignoredActions: n = [],
      ignoredActionPaths: s = ["meta.arg", "meta.baseQueryMeta"],
      ignoredPaths: a = [],
      warnAfter: i = 32,
      ignoreState: l = !1,
      ignoreActions: u = !1,
      disableCache: d = !1
    } = e, c = !d && WeakSet ? /* @__PURE__ */ new WeakSet() : void 0;
    return (f) => (y) => (m) => {
      if (!Eo(m))
        return y(m);
      const p = y(m), b = Po(i, "SerializableStateInvariantMiddleware");
      return !u && !(n.length && n.indexOf(m.type) !== -1) && b.measureTime(() => {
        const g = vn(m, "", t, r, s, c);
        if (g) {
          const {
            keyPath: E,
            value: N
          } = g;
          console.error(`A non-serializable value was detected in an action, in the path: \`${E}\`. Value:`, N, `
Take a look at the logic that dispatched this action: `, m, `
(See https://redux.js.org/faq/actions#why-should-type-be-a-string-or-at-least-serializable-why-should-my-action-types-be-constants)`, `
(To allow non-serializable values see: https://redux-toolkit.js.org/usage/usage-guide#working-with-non-serializable-data)`);
        }
      }), l || (b.measureTime(() => {
        const g = f.getState(), E = vn(g, "", t, r, a, c);
        if (E) {
          const {
            keyPath: N,
            value: _
          } = E;
          console.error(`A non-serializable value was detected in the state, in the path: \`${N}\`. Value:`, _, `
Take a look at the reducer(s) handling this action type: ${m.type}.
(See https://redux.js.org/faq/organizing-state#can-i-put-functions-promises-or-other-non-serializable-items-in-my-store-state)`);
        }
      }), b.warnIfExceeded()), p;
    };
  }
}
function tr(e) {
  return typeof e == "boolean";
}
var kl = () => function(t) {
  const {
    thunk: r = !0,
    immutableCheck: n = !0,
    serializableCheck: s = !0,
    actionCreatorCheck: a = !0
  } = t ?? {};
  let i = new Lo();
  if (r && (tr(r) ? i.push(hl) : i.push(pl(r.extraArgument))), process.env.NODE_ENV !== "production") {
    if (n) {
      let l = {};
      tr(n) || (l = n), i.unshift(wl(l));
    }
    if (s) {
      let l = {};
      tr(s) || (l = s), i.push(El(l));
    }
    if (a) {
      let l = {};
      tr(a) || (l = a), i.unshift(xl(l));
    }
  }
  return i;
}, Nl = "RTK_autoBatch", Ss = (e) => (t) => {
  setTimeout(t, e);
}, jl = (e = {
  type: "raf"
}) => (t) => (...r) => {
  const n = t(...r);
  let s = !0, a = !1, i = !1;
  const l = /* @__PURE__ */ new Set(), u = e.type === "tick" ? queueMicrotask : e.type === "raf" ? (
    // requestAnimationFrame won't exist in SSR environments. Fall back to a vague approximation just to keep from erroring.
    typeof window < "u" && window.requestAnimationFrame ? window.requestAnimationFrame : Ss(10)
  ) : e.type === "callback" ? e.queueNotification : Ss(e.timeout), d = () => {
    i = !1, a && (a = !1, l.forEach((c) => c()));
  };
  return Object.assign({}, n, {
    // Override the base `store.subscribe` method to keep original listeners
    // from running if we're delaying notifications
    subscribe(c) {
      const f = () => s && c(), y = n.subscribe(f);
      return l.add(c), () => {
        y(), l.delete(c);
      };
    },
    // Override the base `store.dispatch` method so that we can check actions
    // for the `shouldAutoBatch` flag and determine if batching is active
    dispatch(c) {
      var f;
      try {
        return s = !((f = c == null ? void 0 : c.meta) != null && f[Nl]), a = !s, a && (i || (i = !0, u(d))), n.dispatch(c);
      } finally {
        s = !0;
      }
    }
  });
}, Sl = (e) => function(r) {
  const {
    autoBatch: n = !0
  } = r ?? {};
  let s = new Lo(e);
  return n && s.push(jl(typeof n == "object" ? n : void 0)), s;
};
function _l(e) {
  const t = kl(), {
    reducer: r = void 0,
    middleware: n,
    devTools: s = !0,
    duplicateMiddlewareCheck: a = !0,
    preloadedState: i = void 0,
    enhancers: l = void 0
  } = e || {};
  let u;
  if (typeof r == "function")
    u = r;
  else if (Bt(r))
    u = wo(r);
  else
    throw new Error(process.env.NODE_ENV === "production" ? Z(1) : "`reducer` is a required argument, and must be a function or an object of functions that can be passed to combineReducers");
  if (process.env.NODE_ENV !== "production" && n && typeof n != "function")
    throw new Error(process.env.NODE_ENV === "production" ? Z(2) : "`middleware` field must be a callback");
  let d;
  if (typeof n == "function") {
    if (d = n(t), process.env.NODE_ENV !== "production" && !Array.isArray(d))
      throw new Error(process.env.NODE_ENV === "production" ? Z(3) : "when using a middleware builder function, an array of middleware must be returned");
  } else
    d = t();
  if (process.env.NODE_ENV !== "production" && d.some((b) => typeof b != "function"))
    throw new Error(process.env.NODE_ENV === "production" ? Z(4) : "each middleware provided to configureStore must be a function");
  if (process.env.NODE_ENV !== "production" && a) {
    let b = /* @__PURE__ */ new Set();
    d.forEach((g) => {
      if (b.has(g))
        throw new Error(process.env.NODE_ENV === "production" ? Z(42) : "Duplicate middleware references found when creating the store. Ensure that each middleware is only included once.");
      b.add(g);
    });
  }
  let c = hr;
  s && (c = ml({
    // Enable capture of stack traces for dispatched Redux actions
    trace: process.env.NODE_ENV !== "production",
    ...typeof s == "object" && s
  }));
  const f = Gi(...d), y = Sl(f);
  if (process.env.NODE_ENV !== "production" && l && typeof l != "function")
    throw new Error(process.env.NODE_ENV === "production" ? Z(5) : "`enhancers` field must be a callback");
  let m = typeof l == "function" ? l(y) : y();
  if (process.env.NODE_ENV !== "production" && !Array.isArray(m))
    throw new Error(process.env.NODE_ENV === "production" ? Z(6) : "`enhancers` callback must return an array");
  if (process.env.NODE_ENV !== "production" && m.some((b) => typeof b != "function"))
    throw new Error(process.env.NODE_ENV === "production" ? Z(7) : "each enhancer provided to configureStore must be a function");
  process.env.NODE_ENV !== "production" && d.length && !m.includes(f) && console.error("middlewares were provided, but middleware enhancer was not included in final enhancers - make sure to call `getDefaultEnhancers`");
  const p = c(...m);
  return vo(u, i, p);
}
function Bo(e) {
  const t = {}, r = [];
  let n;
  const s = {
    addCase(a, i) {
      if (process.env.NODE_ENV !== "production") {
        if (r.length > 0)
          throw new Error(process.env.NODE_ENV === "production" ? Z(26) : "`builder.addCase` should only be called before calling `builder.addMatcher`");
        if (n)
          throw new Error(process.env.NODE_ENV === "production" ? Z(27) : "`builder.addCase` should only be called before calling `builder.addDefaultCase`");
      }
      const l = typeof a == "string" ? a : a.type;
      if (!l)
        throw new Error(process.env.NODE_ENV === "production" ? Z(28) : "`builder.addCase` cannot be called with an empty action type");
      if (l in t)
        throw new Error(process.env.NODE_ENV === "production" ? Z(29) : `\`builder.addCase\` cannot be called with two reducers for the same action type '${l}'`);
      return t[l] = i, s;
    },
    addAsyncThunk(a, i) {
      if (process.env.NODE_ENV !== "production" && n)
        throw new Error(process.env.NODE_ENV === "production" ? Z(43) : "`builder.addAsyncThunk` should only be called before calling `builder.addDefaultCase`");
      return i.pending && (t[a.pending.type] = i.pending), i.rejected && (t[a.rejected.type] = i.rejected), i.fulfilled && (t[a.fulfilled.type] = i.fulfilled), i.settled && r.push({
        matcher: a.settled,
        reducer: i.settled
      }), s;
    },
    addMatcher(a, i) {
      if (process.env.NODE_ENV !== "production" && n)
        throw new Error(process.env.NODE_ENV === "production" ? Z(30) : "`builder.addMatcher` should only be called before calling `builder.addDefaultCase`");
      return r.push({
        matcher: a,
        reducer: i
      }), s;
    },
    addDefaultCase(a) {
      if (process.env.NODE_ENV !== "production" && n)
        throw new Error(process.env.NODE_ENV === "production" ? Z(31) : "`builder.addDefaultCase` can only be called once");
      return n = a, s;
    }
  };
  return e(s), [t, r, n];
}
function Rl(e) {
  return typeof e == "function";
}
function Ol(e, t) {
  if (process.env.NODE_ENV !== "production" && typeof t == "object")
    throw new Error(process.env.NODE_ENV === "production" ? Z(8) : "The object notation for `createReducer` has been removed. Please use the 'builder callback' notation instead: https://redux-toolkit.js.org/api/createReducer");
  let [r, n, s] = Bo(t), a;
  if (Rl(e))
    a = () => js(e());
  else {
    const l = js(e);
    a = () => l;
  }
  function i(l = a(), u) {
    let d = [r[u.type], ...n.filter(({
      matcher: c
    }) => c(u)).map(({
      reducer: c
    }) => c)];
    return d.filter((c) => !!c).length === 0 && (d = [s]), d.reduce((c, f) => {
      if (f)
        if (Ue(c)) {
          const m = f(c, u);
          return m === void 0 ? c : m;
        } else {
          if (Re(c))
            return Ao(c, (y) => f(y, u));
          {
            const y = f(c, u);
            if (y === void 0) {
              if (c === null)
                return c;
              throw Error("A case reducer on a non-draftable value must not return undefined");
            }
            return y;
          }
        }
      return c;
    }, l);
  }
  return i.getInitialState = a, i;
}
var Tl = (e, t) => Mo(e) ? e.match(t) : e(t);
function Cl(...e) {
  return (t) => e.some((r) => Tl(r, t));
}
var Al = "ModuleSymbhasOwnPr-0123456789ABCDEFGHNRVfgctiUvz_KqYTJkLxpZXIjQW", Dl = (e = 21) => {
  let t = "", r = e;
  for (; r--; )
    t += Al[Math.random() * 64 | 0];
  return t;
}, Ml = ["name", "message", "stack", "code"], rn = class {
  constructor(e, t) {
    /*
    type-only property to distinguish between RejectWithValue and FulfillWithMeta
    does not exist at runtime
    */
    Gr(this, "_type");
    this.payload = e, this.meta = t;
  }
}, _s = class {
  constructor(e, t) {
    /*
    type-only property to distinguish between RejectWithValue and FulfillWithMeta
    does not exist at runtime
    */
    Gr(this, "_type");
    this.payload = e, this.meta = t;
  }
}, Pl = (e) => {
  if (typeof e == "object" && e !== null) {
    const t = {};
    for (const r of Ml)
      typeof e[r] == "string" && (t[r] = e[r]);
    return t;
  }
  return {
    message: String(e)
  };
}, Rs = "External signal was aborted", Ar = /* @__PURE__ */ (() => {
  function e(t, r, n) {
    const s = At(t + "/fulfilled", (u, d, c, f) => ({
      payload: u,
      meta: {
        ...f || {},
        arg: c,
        requestId: d,
        requestStatus: "fulfilled"
      }
    })), a = At(t + "/pending", (u, d, c) => ({
      payload: void 0,
      meta: {
        ...c || {},
        arg: d,
        requestId: u,
        requestStatus: "pending"
      }
    })), i = At(t + "/rejected", (u, d, c, f, y) => ({
      payload: f,
      error: (n && n.serializeError || Pl)(u || "Rejected"),
      meta: {
        ...y || {},
        arg: c,
        requestId: d,
        rejectedWithValue: !!f,
        requestStatus: "rejected",
        aborted: (u == null ? void 0 : u.name) === "AbortError",
        condition: (u == null ? void 0 : u.name) === "ConditionError"
      }
    }));
    function l(u, {
      signal: d
    } = {}) {
      return (c, f, y) => {
        const m = n != null && n.idGenerator ? n.idGenerator(u) : Dl(), p = new AbortController();
        let b, g;
        function E(_) {
          g = _, p.abort();
        }
        d && (d.aborted ? E(Rs) : d.addEventListener("abort", () => E(Rs), {
          once: !0
        }));
        const N = async function() {
          var S, A;
          let _;
          try {
            let D = (S = n == null ? void 0 : n.condition) == null ? void 0 : S.call(n, u, {
              getState: f,
              extra: y
            });
            if (Il(D) && (D = await D), D === !1 || p.signal.aborted)
              throw {
                name: "ConditionError",
                message: "Aborted due to condition callback returning false."
              };
            const U = new Promise((T, M) => {
              b = () => {
                M({
                  name: "AbortError",
                  message: g || "Aborted"
                });
              }, p.signal.addEventListener("abort", b, {
                once: !0
              });
            });
            c(a(m, u, (A = n == null ? void 0 : n.getPendingMeta) == null ? void 0 : A.call(n, {
              requestId: m,
              arg: u
            }, {
              getState: f,
              extra: y
            }))), _ = await Promise.race([U, Promise.resolve(r(u, {
              dispatch: c,
              getState: f,
              extra: y,
              requestId: m,
              signal: p.signal,
              abort: E,
              rejectWithValue: (T, M) => new rn(T, M),
              fulfillWithValue: (T, M) => new _s(T, M)
            })).then((T) => {
              if (T instanceof rn)
                throw T;
              return T instanceof _s ? s(T.payload, m, u, T.meta) : s(T, m, u);
            })]);
          } catch (D) {
            _ = D instanceof rn ? i(null, m, u, D.payload, D.meta) : i(D, m, u);
          } finally {
            b && p.signal.removeEventListener("abort", b);
          }
          return n && !n.dispatchConditionRejection && i.match(_) && _.meta.condition || c(_), _;
        }();
        return Object.assign(N, {
          abort: E,
          requestId: m,
          arg: u,
          unwrap() {
            return N.then(Ll);
          }
        });
      };
    }
    return Object.assign(l, {
      pending: a,
      rejected: i,
      fulfilled: s,
      settled: Cl(i, s),
      typePrefix: t
    });
  }
  return e.withTypes = () => e, e;
})();
function Ll(e) {
  if (e.meta && e.meta.rejectedWithValue)
    throw e.payload;
  if (e.error)
    throw e.error;
  return e.payload;
}
function Il(e) {
  return e !== null && typeof e == "object" && typeof e.then == "function";
}
var Fl = /* @__PURE__ */ Symbol.for("rtk-slice-createasyncthunk");
function Ul(e, t) {
  return `${e}/${t}`;
}
function $l({
  creators: e
} = {}) {
  var r;
  const t = (r = e == null ? void 0 : e.asyncThunk) == null ? void 0 : r[Fl];
  return function(s) {
    const {
      name: a,
      reducerPath: i = a
    } = s;
    if (!a)
      throw new Error(process.env.NODE_ENV === "production" ? Z(11) : "`name` is a required option for createSlice");
    typeof process < "u" && process.env.NODE_ENV === "development" && s.initialState === void 0 && console.error("You must provide an `initialState` value that is not `undefined`. You may have misspelled `initialState`");
    const l = (typeof s.reducers == "function" ? s.reducers(Vl()) : s.reducers) || {}, u = Object.keys(l), d = {
      sliceCaseReducersByName: {},
      sliceCaseReducersByType: {},
      actionCreators: {},
      sliceMatchers: []
    }, c = {
      addCase(w, S) {
        const A = typeof w == "string" ? w : w.type;
        if (!A)
          throw new Error(process.env.NODE_ENV === "production" ? Z(12) : "`context.addCase` cannot be called with an empty action type");
        if (A in d.sliceCaseReducersByType)
          throw new Error(process.env.NODE_ENV === "production" ? Z(13) : "`context.addCase` cannot be called with two reducers for the same action type: " + A);
        return d.sliceCaseReducersByType[A] = S, c;
      },
      addMatcher(w, S) {
        return d.sliceMatchers.push({
          matcher: w,
          reducer: S
        }), c;
      },
      exposeAction(w, S) {
        return d.actionCreators[w] = S, c;
      },
      exposeCaseReducer(w, S) {
        return d.sliceCaseReducersByName[w] = S, c;
      }
    };
    u.forEach((w) => {
      const S = l[w], A = {
        reducerName: w,
        type: Ul(a, w),
        createNotation: typeof s.reducers == "function"
      };
      Wl(S) ? ql(A, S, c, t) : zl(A, S, c);
    });
    function f() {
      if (process.env.NODE_ENV !== "production" && typeof s.extraReducers == "object")
        throw new Error(process.env.NODE_ENV === "production" ? Z(14) : "The object notation for `createSlice.extraReducers` has been removed. Please use the 'builder callback' notation instead: https://redux-toolkit.js.org/api/createSlice");
      const [w = {}, S = [], A = void 0] = typeof s.extraReducers == "function" ? Bo(s.extraReducers) : [s.extraReducers], D = {
        ...w,
        ...d.sliceCaseReducersByType
      };
      return Ol(s.initialState, (U) => {
        for (let T in D)
          U.addCase(T, D[T]);
        for (let T of d.sliceMatchers)
          U.addMatcher(T.matcher, T.reducer);
        for (let T of S)
          U.addMatcher(T.matcher, T.reducer);
        A && U.addDefaultCase(A);
      });
    }
    const y = (w) => w, m = /* @__PURE__ */ new Map(), p = /* @__PURE__ */ new WeakMap();
    let b;
    function g(w, S) {
      return b || (b = f()), b(w, S);
    }
    function E() {
      return b || (b = f()), b.getInitialState();
    }
    function N(w, S = !1) {
      function A(U) {
        let T = U[w];
        if (typeof T > "u") {
          if (S)
            T = er(p, A, E);
          else if (process.env.NODE_ENV !== "production")
            throw new Error(process.env.NODE_ENV === "production" ? Z(15) : "selectSlice returned undefined for an uninjected slice reducer");
        }
        return T;
      }
      function D(U = y) {
        const T = er(m, S, () => /* @__PURE__ */ new WeakMap());
        return er(T, U, () => {
          const M = {};
          for (const [C, $] of Object.entries(s.selectors ?? {}))
            M[C] = Bl($, U, () => er(p, U, E), S);
          return M;
        });
      }
      return {
        reducerPath: w,
        getSelectors: D,
        get selectors() {
          return D(A);
        },
        selectSlice: A
      };
    }
    const _ = {
      name: a,
      reducer: g,
      actions: d.actionCreators,
      caseReducers: d.sliceCaseReducersByName,
      getInitialState: E,
      ...N(i),
      injectInto(w, {
        reducerPath: S,
        ...A
      } = {}) {
        const D = S ?? i;
        return w.inject({
          reducerPath: D,
          reducer: g
        }, A), {
          ..._,
          ...N(D, !0)
        };
      }
    };
    return _;
  };
}
function Bl(e, t, r, n) {
  function s(a, ...i) {
    let l = t(a);
    if (typeof l > "u") {
      if (n)
        l = r();
      else if (process.env.NODE_ENV !== "production")
        throw new Error(process.env.NODE_ENV === "production" ? Z(16) : "selectState returned undefined for an uninjected slice reducer");
    }
    return e(l, ...i);
  }
  return s.unwrapped = e, s;
}
var Vo = /* @__PURE__ */ $l();
function Vl() {
  function e(t, r) {
    return {
      _reducerDefinitionType: "asyncThunk",
      payloadCreator: t,
      ...r
    };
  }
  return e.withTypes = () => e, {
    reducer(t) {
      return Object.assign({
        // hack so the wrapping function has the same name as the original
        // we need to create a wrapper so the `reducerDefinitionType` is not assigned to the original
        [t.name](...r) {
          return t(...r);
        }
      }[t.name], {
        _reducerDefinitionType: "reducer"
        /* reducer */
      });
    },
    preparedReducer(t, r) {
      return {
        _reducerDefinitionType: "reducerWithPrepare",
        prepare: t,
        reducer: r
      };
    },
    asyncThunk: e
  };
}
function zl({
  type: e,
  reducerName: t,
  createNotation: r
}, n, s) {
  let a, i;
  if ("reducer" in n) {
    if (r && !Hl(n))
      throw new Error(process.env.NODE_ENV === "production" ? Z(17) : "Please use the `create.preparedReducer` notation for prepared action creators with the `create` notation.");
    a = n.reducer, i = n.prepare;
  } else
    a = n;
  s.addCase(e, a).exposeCaseReducer(t, a).exposeAction(t, i ? At(e, i) : At(e));
}
function Wl(e) {
  return e._reducerDefinitionType === "asyncThunk";
}
function Hl(e) {
  return e._reducerDefinitionType === "reducerWithPrepare";
}
function ql({
  type: e,
  reducerName: t
}, r, n, s) {
  if (!s)
    throw new Error(process.env.NODE_ENV === "production" ? Z(18) : "Cannot use `create.asyncThunk` in the built-in `createSlice`. Use `buildCreateSlice({ creators: { asyncThunk: asyncThunkCreator } })` to create a customised version of `createSlice`.");
  const {
    payloadCreator: a,
    fulfilled: i,
    pending: l,
    rejected: u,
    settled: d,
    options: c
  } = r, f = s(e, a, c);
  n.exposeAction(t, f), i && n.addCase(f.fulfilled, i), l && n.addCase(f.pending, l), u && n.addCase(f.rejected, u), d && n.addMatcher(f.settled, d), n.exposeCaseReducer(t, {
    fulfilled: i || rr,
    pending: l || rr,
    rejected: u || rr,
    settled: d || rr
  });
}
function rr() {
}
function Z(e) {
  return `Minified Redux Toolkit error #${e}; visit https://redux-toolkit.js.org/Errors?code=${e} for the full message or use the non-minified dev environment for full errors. `;
}
class Tt extends Error {
}
Tt.prototype.name = "InvalidTokenError";
function Kl(e) {
  return decodeURIComponent(atob(e).replace(/(.)/g, (t, r) => {
    let n = r.charCodeAt(0).toString(16).toUpperCase();
    return n.length < 2 && (n = "0" + n), "%" + n;
  }));
}
function Jl(e) {
  let t = e.replace(/-/g, "+").replace(/_/g, "/");
  switch (t.length % 4) {
    case 0:
      break;
    case 2:
      t += "==";
      break;
    case 3:
      t += "=";
      break;
    default:
      throw new Error("base64 string is not of the correct length");
  }
  try {
    return Kl(t);
  } catch {
    return atob(t);
  }
}
function Ql(e, t) {
  if (typeof e != "string")
    throw new Tt("Invalid token specified: must be a string");
  t || (t = {});
  const r = t.header === !0 ? 0 : 1, n = e.split(".")[r];
  if (typeof n != "string")
    throw new Tt(`Invalid token specified: missing part #${r + 1}`);
  let s;
  try {
    s = Jl(n);
  } catch (a) {
    throw new Tt(`Invalid token specified: invalid base64 for part #${r + 1} (${a.message})`);
  }
  try {
    return JSON.parse(s);
  } catch (a) {
    throw new Tt(`Invalid token specified: invalid json for part #${r + 1} (${a.message})`);
  }
}
const Gl = {
  loginEndpoint: "/api/auth/login",
  logoutEndpoint: "/api/auth/logout",
  refreshEndpoint: "/api/auth/refresh",
  userEndpoint: "/api/auth/me",
  tokenKey: "neo_access_token",
  refreshTokenKey: "neo_refresh_token",
  sessionTimeout: 30 * 60 * 1e3,
  // 30 minutes
  autoRefreshBuffer: 5 * 60 * 1e3,
  // 5 minutes before expiry
  persistSession: !0,
  redirectAfterLogin: "/",
  redirectAfterLogout: "/login",
  unauthorizedRedirect: "/login"
}, zo = {
  baseURL: "",
  timeout: 3e4,
  // 30 seconds
  withCredentials: !0,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
    "X-Requested-With": "XMLHttpRequest"
  },
  retryAttempts: 3,
  retryDelay: 1e3
}, Os = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  CONFLICT: 409,
  UNPROCESSABLE_ENTITY: 422,
  TOO_MANY_REQUESTS: 429,
  INTERNAL_SERVER_ERROR: 500,
  BAD_GATEWAY: 502,
  SERVICE_UNAVAILABLE: 503,
  GATEWAY_TIMEOUT: 504
}, nn = {
  NETWORK_ERROR: "NETWORK_ERROR",
  TIMEOUT: "TIMEOUT",
  UNAUTHORIZED: "UNAUTHORIZED",
  FORBIDDEN: "FORBIDDEN",
  NOT_FOUND: "NOT_FOUND",
  VALIDATION_ERROR: "VALIDATION_ERROR",
  SERVER_ERROR: "SERVER_ERROR",
  UNKNOWN_ERROR: "UNKNOWN_ERROR",
  TOKEN_EXPIRED: "TOKEN_EXPIRED",
  INVALID_TOKEN: "INVALID_TOKEN",
  SESSION_EXPIRED: "SESSION_EXPIRED"
}, Q = {
  ACCESS_TOKEN: "neo_access_token",
  REFRESH_TOKEN: "neo_refresh_token",
  USER: "neo_user",
  THEME: "neo_theme",
  LANGUAGE: "neo_language",
  SIDEBAR_COLLAPSED: "neo_sidebar_collapsed",
  TABLE_SETTINGS: "neo_table_settings"
}, oe = {
  AUTH_STATE_CHANGED: "neo:auth:state_changed",
  TOKEN_REFRESHED: "neo:auth:token_refreshed",
  SESSION_EXPIRED: "neo:auth:session_expired",
  UNAUTHORIZED: "neo:auth:unauthorized",
  PERMISSION_DENIED: "neo:auth:permission_denied",
  NETWORK_ERROR: "neo:network:error",
  SERVER_ERROR: "neo:server:error"
}, rf = {
  USER: ["user"],
  USERS: ["users"],
  ROLES: ["roles"],
  PERMISSIONS: ["permissions"],
  SESSIONS: ["sessions"]
}, nf = {
  PAGE: 1,
  PAGE_SIZE: 20,
  PAGE_SIZE_OPTIONS: [10, 20, 50, 100],
  MAX_PAGE_SIZE: 100
}, Yl = {
  user: null,
  accessToken: null,
  refreshToken: null,
  expiresAt: null,
  isAuthenticated: !1,
  isLoading: !1,
  error: null,
  lastActivity: null
}, Xl = () => {
  try {
    const e = localStorage.getItem(Q.ACCESS_TOKEN), t = localStorage.getItem(Q.REFRESH_TOKEN), r = localStorage.getItem(Q.USER);
    if (!e || !r)
      return {};
    const s = Ql(e).exp * 1e3;
    return Date.now() >= s ? (localStorage.removeItem(Q.ACCESS_TOKEN), localStorage.removeItem(Q.REFRESH_TOKEN), localStorage.removeItem(Q.USER), {}) : {
      user: JSON.parse(r),
      accessToken: e,
      refreshToken: t,
      expiresAt: s,
      isAuthenticated: !0,
      lastActivity: Date.now()
    };
  } catch {
    return {};
  }
}, je = (e) => {
  e.accessToken && e.user ? (localStorage.setItem(Q.ACCESS_TOKEN, e.accessToken), e.refreshToken && localStorage.setItem(Q.REFRESH_TOKEN, e.refreshToken), localStorage.setItem(Q.USER, JSON.stringify(e.user))) : (localStorage.removeItem(Q.ACCESS_TOKEN), localStorage.removeItem(Q.REFRESH_TOKEN), localStorage.removeItem(Q.USER));
}, $e = (e, t) => {
  window.dispatchEvent(new CustomEvent(e, { detail: t }));
}, Dt = Ar("auth/login", async ({ credentials: e, apiClient: t }, { rejectWithValue: r }) => {
  try {
    return (await t.post("/api/auth/login", e)).data;
  } catch (n) {
    const s = n instanceof Error ? n.message : "Login failed";
    return r(s);
  }
}), ir = Ar("auth/logout", async (e) => {
  try {
    e != null && e.apiClient && await e.apiClient.post("/api/auth/logout");
  } catch (t) {
    console.error("Logout API error:", t);
  }
}), vr = Ar("auth/refreshToken", async ({ apiClient: e }, { getState: t, rejectWithValue: r }) => {
  try {
    const s = t().auth.refreshToken;
    return s ? (await e.post("/api/auth/refresh", { refreshToken: s })).data : r("No refresh token available");
  } catch (n) {
    const s = n instanceof Error ? n.message : "Token refresh failed";
    return r(s);
  }
}), sn = Ar("auth/fetchUser", async ({ apiClient: e }, { rejectWithValue: t }) => {
  try {
    return (await e.get("/api/auth/me")).data;
  } catch (r) {
    const n = r instanceof Error ? r.message : "Failed to fetch user";
    return t(n);
  }
}), Wo = Vo({
  name: "auth",
  initialState: { ...Yl, ...Xl() },
  reducers: {
    /**
     * Set user manually
     */
    setUser: (e, t) => {
      e.user = t.payload, e.isAuthenticated = !!t.payload, je(e), $e(oe.AUTH_STATE_CHANGED, { user: t.payload });
    },
    /**
     * Update user partially
     */
    updateUser: (e, t) => {
      e.user && (e.user = { ...e.user, ...t.payload }, je(e));
    },
    /**
     * Set tokens
     */
    setTokens: (e, t) => {
      const { accessToken: r, refreshToken: n, expiresIn: s } = t.payload;
      e.accessToken = r, n && (e.refreshToken = n), e.expiresAt = Date.now() + s * 1e3, je(e), $e(oe.TOKEN_REFRESHED);
    },
    /**
     * Update last activity timestamp
     */
    updateLastActivity: (e) => {
      e.lastActivity = Date.now();
    },
    /**
     * Clear auth state (for logout)
     */
    clearAuth: (e) => {
      e.user = null, e.accessToken = null, e.refreshToken = null, e.expiresAt = null, e.isAuthenticated = !1, e.error = null, e.lastActivity = null, je(e), $e(oe.AUTH_STATE_CHANGED, { user: null });
    },
    /**
     * Set error
     */
    setError: (e, t) => {
      e.error = t.payload;
    },
    /**
     * Clear error
     */
    clearError: (e) => {
      e.error = null;
    },
    /**
     * Handle session expiry
     */
    sessionExpired: (e) => {
      e.user = null, e.accessToken = null, e.refreshToken = null, e.expiresAt = null, e.isAuthenticated = !1, e.error = "Session expired. Please login again.", je(e), $e(oe.SESSION_EXPIRED);
    }
  },
  extraReducers: (e) => {
    e.addCase(Dt.pending, (t) => {
      t.isLoading = !0, t.error = null;
    }).addCase(Dt.fulfilled, (t, r) => {
      const { user: n, accessToken: s, refreshToken: a, expiresIn: i } = r.payload;
      t.user = n, t.accessToken = s, t.refreshToken = a, t.expiresAt = Date.now() + i * 1e3, t.isAuthenticated = !0, t.isLoading = !1, t.error = null, t.lastActivity = Date.now(), je(t), $e(oe.AUTH_STATE_CHANGED, { user: n });
    }).addCase(Dt.rejected, (t, r) => {
      t.isLoading = !1, t.error = r.payload || "Login failed", t.isAuthenticated = !1;
    }), e.addCase(ir.pending, (t) => {
      t.isLoading = !0;
    }).addCase(ir.fulfilled, (t) => {
      t.user = null, t.accessToken = null, t.refreshToken = null, t.expiresAt = null, t.isAuthenticated = !1, t.isLoading = !1, t.error = null, t.lastActivity = null, je(t), $e(oe.AUTH_STATE_CHANGED, { user: null });
    }).addCase(ir.rejected, (t) => {
      t.user = null, t.accessToken = null, t.refreshToken = null, t.expiresAt = null, t.isAuthenticated = !1, t.isLoading = !1, je(t);
    }), e.addCase(vr.fulfilled, (t, r) => {
      const { accessToken: n, refreshToken: s, expiresIn: a } = r.payload;
      t.accessToken = n, t.refreshToken = s, t.expiresAt = Date.now() + a * 1e3, je(t), $e(oe.TOKEN_REFRESHED);
    }).addCase(vr.rejected, (t, r) => {
      t.user = null, t.accessToken = null, t.refreshToken = null, t.expiresAt = null, t.isAuthenticated = !1, t.error = r.payload || "Session expired", je(t), $e(oe.SESSION_EXPIRED);
    }), e.addCase(sn.pending, (t) => {
      t.isLoading = !0;
    }).addCase(sn.fulfilled, (t, r) => {
      t.user = r.payload, t.isLoading = !1, je(t);
    }).addCase(sn.rejected, (t, r) => {
      t.isLoading = !1, t.error = r.payload || "Failed to fetch user";
    });
  }
}), {
  setUser: sf,
  updateUser: Zl,
  setTokens: of,
  updateLastActivity: ec,
  clearAuth: tc,
  setError: af,
  clearError: lf,
  sessionExpired: cf
} = Wo.actions, Vn = (e) => e.auth.user, Ho = (e) => e.auth.isAuthenticated, qo = (e) => e.auth.isLoading, Ko = (e) => e.auth.error, Jo = (e) => e.auth.accessToken, Qo = (e) => e.auth.expiresAt, df = (e) => (t) => {
  const r = t.auth.user;
  return r ? r.isAdmin ? !0 : r.permissions.some((n) => n.id === e || n.name === e) : !1;
}, uf = (e) => (t) => {
  const r = t.auth.user;
  return r ? r.isAdmin ? !0 : r.roles.includes(e) : !1;
}, rc = Wo.reducer, Go = {
  mode: "light",
  primaryColor: "#0ea5e9",
  borderRadius: "md",
  fontFamily: "Vazirmatn",
  direction: "rtl"
}, nc = () => {
  try {
    const e = localStorage.getItem(Q.THEME), t = localStorage.getItem(Q.LANGUAGE), r = localStorage.getItem(Q.SIDEBAR_COLLAPSED);
    return {
      theme: e ? JSON.parse(e) : Go,
      language: t || "fa",
      sidebarCollapsed: r === "true"
    };
  } catch {
    return {};
  }
}, sc = {
  theme: Go,
  language: "fa",
  sidebarCollapsed: !1,
  sidebarMobileOpen: !1,
  notifications: [],
  isLoading: !1,
  loadingMessage: null,
  breadcrumbs: [],
  pageTitle: null,
  ...nc()
};
let oc = 0;
const ac = () => `notification_${++oc}_${Date.now()}`, Yo = Vo({
  name: "ui",
  initialState: sc,
  reducers: {
    /**
     * Set theme
     */
    setTheme: (e, t) => {
      e.theme = { ...e.theme, ...t.payload }, localStorage.setItem(Q.THEME, JSON.stringify(e.theme));
      const r = document.documentElement;
      r.setAttribute("data-theme", e.theme.mode), r.setAttribute("dir", e.theme.direction), r.style.setProperty("--primary-color", e.theme.primaryColor);
    },
    /**
     * Toggle theme mode
     */
    toggleThemeMode: (e) => {
      e.theme.mode = e.theme.mode === "light" ? "dark" : "light", localStorage.setItem(Q.THEME, JSON.stringify(e.theme)), document.documentElement.setAttribute("data-theme", e.theme.mode);
    },
    /**
     * Set language
     */
    setLanguage: (e, t) => {
      e.language = t.payload, localStorage.setItem(Q.LANGUAGE, t.payload);
      const r = ["fa", "ar", "he"].includes(t.payload);
      e.theme.direction = r ? "rtl" : "ltr", document.documentElement.setAttribute("dir", e.theme.direction), document.documentElement.setAttribute("lang", t.payload);
    },
    /**
     * Toggle sidebar collapsed state
     */
    toggleSidebar: (e) => {
      e.sidebarCollapsed = !e.sidebarCollapsed, localStorage.setItem(Q.SIDEBAR_COLLAPSED, String(e.sidebarCollapsed));
    },
    /**
     * Set sidebar collapsed state
     */
    setSidebarCollapsed: (e, t) => {
      e.sidebarCollapsed = t.payload, localStorage.setItem(Q.SIDEBAR_COLLAPSED, String(t.payload));
    },
    /**
     * Toggle mobile sidebar
     */
    toggleMobileSidebar: (e) => {
      e.sidebarMobileOpen = !e.sidebarMobileOpen;
    },
    /**
     * Set mobile sidebar state
     */
    setMobileSidebarOpen: (e, t) => {
      e.sidebarMobileOpen = t.payload;
    },
    /**
     * Add notification
     */
    addNotification: (e, t) => {
      const r = {
        ...t.payload,
        id: ac()
      };
      e.notifications.push(r), r.duration && r.duration > 0 && setTimeout(() => {
      }, r.duration);
    },
    /**
     * Remove notification
     */
    removeNotification: (e, t) => {
      e.notifications = e.notifications.filter(
        (r) => r.id !== t.payload
      );
    },
    /**
     * Clear all notifications
     */
    clearNotifications: (e) => {
      e.notifications = [];
    },
    /**
     * Show loading overlay
     */
    showLoading: (e, t) => {
      e.isLoading = !0, e.loadingMessage = t.payload || null;
    },
    /**
     * Hide loading overlay
     */
    hideLoading: (e) => {
      e.isLoading = !1, e.loadingMessage = null;
    },
    /**
     * Set breadcrumbs
     */
    setBreadcrumbs: (e, t) => {
      e.breadcrumbs = t.payload;
    },
    /**
     * Set page title
     */
    setPageTitle: (e, t) => {
      e.pageTitle = t.payload, t.payload && (document.title = `${t.payload} | Neo BPMS`);
    }
  }
}), {
  setTheme: ff,
  toggleThemeMode: hf,
  setLanguage: pf,
  toggleSidebar: mf,
  setSidebarCollapsed: gf,
  toggleMobileSidebar: yf,
  setMobileSidebarOpen: xf,
  addNotification: bf,
  removeNotification: vf,
  clearNotifications: wf,
  showLoading: Ef,
  hideLoading: kf,
  setBreadcrumbs: Nf,
  setPageTitle: jf
} = Yo.actions, Sf = (e) => e.ui.theme, _f = (e) => e.ui.language, Rf = (e) => e.ui.sidebarCollapsed, Of = (e) => e.ui.sidebarMobileOpen, Tf = (e) => e.ui.notifications, Cf = (e) => e.ui.isLoading, Af = (e) => e.ui.loadingMessage, Df = (e) => e.ui.breadcrumbs, Mf = (e) => e.ui.pageTitle, ic = Yo.reducer, lc = wo({
  auth: rc,
  ui: ic
}), cc = (e) => (t) => (r) => {
  if (process.env.NODE_ENV === "development") {
    console.group(r.type || "unknown"), console.log("Previous State:", e.getState()), console.log("Action:", r);
    const s = t(r);
    return console.log("Next State:", e.getState()), console.groupEnd(), s;
  }
  return t(r);
}, Ts = () => (e) => (t) => {
  try {
    return e(t);
  } catch (r) {
    throw console.error("Redux error:", r), r;
  }
}, dc = (e) => _l({
  reducer: lc,
  preloadedState: e,
  middleware: (t) => {
    const r = t({
      serializableCheck: {
        // Ignore these paths in the state for serialization check
        ignoredPaths: ["auth.user.metadata"],
        ignoredActions: ["auth/setUser"]
      },
      thunk: {
        extraArgument: void 0
      }
    });
    return process.env.NODE_ENV === "development" ? r.concat(cc, Ts) : r.concat(Ts);
  },
  devTools: process.env.NODE_ENV === "development"
}), zn = dc(), uc = () => di(), xe = ui, Pf = () => zn.getState(), Lf = zn.dispatch;
function Xo(e, t) {
  return function() {
    return e.apply(t, arguments);
  };
}
const { toString: fc } = Object.prototype, { getPrototypeOf: Wn } = Object, { iterator: Dr, toStringTag: Zo } = Symbol, Mr = /* @__PURE__ */ ((e) => (t) => {
  const r = fc.call(t);
  return e[r] || (e[r] = r.slice(8, -1).toLowerCase());
})(/* @__PURE__ */ Object.create(null)), Oe = (e) => (e = e.toLowerCase(), (t) => Mr(t) === e), Pr = (e) => (t) => typeof t === e, { isArray: vt } = Array, xt = Pr("undefined");
function zt(e) {
  return e !== null && !xt(e) && e.constructor !== null && !xt(e.constructor) && ge(e.constructor.isBuffer) && e.constructor.isBuffer(e);
}
const ea = Oe("ArrayBuffer");
function hc(e) {
  let t;
  return typeof ArrayBuffer < "u" && ArrayBuffer.isView ? t = ArrayBuffer.isView(e) : t = e && e.buffer && ea(e.buffer), t;
}
const pc = Pr("string"), ge = Pr("function"), ta = Pr("number"), Wt = (e) => e !== null && typeof e == "object", mc = (e) => e === !0 || e === !1, lr = (e) => {
  if (Mr(e) !== "object")
    return !1;
  const t = Wn(e);
  return (t === null || t === Object.prototype || Object.getPrototypeOf(t) === null) && !(Zo in e) && !(Dr in e);
}, gc = (e) => {
  if (!Wt(e) || zt(e))
    return !1;
  try {
    return Object.keys(e).length === 0 && Object.getPrototypeOf(e) === Object.prototype;
  } catch {
    return !1;
  }
}, yc = Oe("Date"), xc = Oe("File"), bc = Oe("Blob"), vc = Oe("FileList"), wc = (e) => Wt(e) && ge(e.pipe), Ec = (e) => {
  let t;
  return e && (typeof FormData == "function" && e instanceof FormData || ge(e.append) && ((t = Mr(e)) === "formdata" || // detect form-data instance
  t === "object" && ge(e.toString) && e.toString() === "[object FormData]"));
}, kc = Oe("URLSearchParams"), [Nc, jc, Sc, _c] = ["ReadableStream", "Request", "Response", "Headers"].map(Oe), Rc = (e) => e.trim ? e.trim() : e.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "");
function Ht(e, t, { allOwnKeys: r = !1 } = {}) {
  if (e === null || typeof e > "u")
    return;
  let n, s;
  if (typeof e != "object" && (e = [e]), vt(e))
    for (n = 0, s = e.length; n < s; n++)
      t.call(null, e[n], n, e);
  else {
    if (zt(e))
      return;
    const a = r ? Object.getOwnPropertyNames(e) : Object.keys(e), i = a.length;
    let l;
    for (n = 0; n < i; n++)
      l = a[n], t.call(null, e[l], l, e);
  }
}
function ra(e, t) {
  if (zt(e))
    return null;
  t = t.toLowerCase();
  const r = Object.keys(e);
  let n = r.length, s;
  for (; n-- > 0; )
    if (s = r[n], t === s.toLowerCase())
      return s;
  return null;
}
const Ge = typeof globalThis < "u" ? globalThis : typeof self < "u" ? self : typeof window < "u" ? window : global, na = (e) => !xt(e) && e !== Ge;
function wn() {
  const { caseless: e, skipUndefined: t } = na(this) && this || {}, r = {}, n = (s, a) => {
    const i = e && ra(r, a) || a;
    lr(r[i]) && lr(s) ? r[i] = wn(r[i], s) : lr(s) ? r[i] = wn({}, s) : vt(s) ? r[i] = s.slice() : (!t || !xt(s)) && (r[i] = s);
  };
  for (let s = 0, a = arguments.length; s < a; s++)
    arguments[s] && Ht(arguments[s], n);
  return r;
}
const Oc = (e, t, r, { allOwnKeys: n } = {}) => (Ht(t, (s, a) => {
  r && ge(s) ? e[a] = Xo(s, r) : e[a] = s;
}, { allOwnKeys: n }), e), Tc = (e) => (e.charCodeAt(0) === 65279 && (e = e.slice(1)), e), Cc = (e, t, r, n) => {
  e.prototype = Object.create(t.prototype, n), e.prototype.constructor = e, Object.defineProperty(e, "super", {
    value: t.prototype
  }), r && Object.assign(e.prototype, r);
}, Ac = (e, t, r, n) => {
  let s, a, i;
  const l = {};
  if (t = t || {}, e == null) return t;
  do {
    for (s = Object.getOwnPropertyNames(e), a = s.length; a-- > 0; )
      i = s[a], (!n || n(i, e, t)) && !l[i] && (t[i] = e[i], l[i] = !0);
    e = r !== !1 && Wn(e);
  } while (e && (!r || r(e, t)) && e !== Object.prototype);
  return t;
}, Dc = (e, t, r) => {
  e = String(e), (r === void 0 || r > e.length) && (r = e.length), r -= t.length;
  const n = e.indexOf(t, r);
  return n !== -1 && n === r;
}, Mc = (e) => {
  if (!e) return null;
  if (vt(e)) return e;
  let t = e.length;
  if (!ta(t)) return null;
  const r = new Array(t);
  for (; t-- > 0; )
    r[t] = e[t];
  return r;
}, Pc = /* @__PURE__ */ ((e) => (t) => e && t instanceof e)(typeof Uint8Array < "u" && Wn(Uint8Array)), Lc = (e, t) => {
  const n = (e && e[Dr]).call(e);
  let s;
  for (; (s = n.next()) && !s.done; ) {
    const a = s.value;
    t.call(e, a[0], a[1]);
  }
}, Ic = (e, t) => {
  let r;
  const n = [];
  for (; (r = e.exec(t)) !== null; )
    n.push(r);
  return n;
}, Fc = Oe("HTMLFormElement"), Uc = (e) => e.toLowerCase().replace(
  /[-_\s]([a-z\d])(\w*)/g,
  function(r, n, s) {
    return n.toUpperCase() + s;
  }
), Cs = (({ hasOwnProperty: e }) => (t, r) => e.call(t, r))(Object.prototype), $c = Oe("RegExp"), sa = (e, t) => {
  const r = Object.getOwnPropertyDescriptors(e), n = {};
  Ht(r, (s, a) => {
    let i;
    (i = t(s, a, e)) !== !1 && (n[a] = i || s);
  }), Object.defineProperties(e, n);
}, Bc = (e) => {
  sa(e, (t, r) => {
    if (ge(e) && ["arguments", "caller", "callee"].indexOf(r) !== -1)
      return !1;
    const n = e[r];
    if (ge(n)) {
      if (t.enumerable = !1, "writable" in t) {
        t.writable = !1;
        return;
      }
      t.set || (t.set = () => {
        throw Error("Can not rewrite read-only method '" + r + "'");
      });
    }
  });
}, Vc = (e, t) => {
  const r = {}, n = (s) => {
    s.forEach((a) => {
      r[a] = !0;
    });
  };
  return vt(e) ? n(e) : n(String(e).split(t)), r;
}, zc = () => {
}, Wc = (e, t) => e != null && Number.isFinite(e = +e) ? e : t;
function Hc(e) {
  return !!(e && ge(e.append) && e[Zo] === "FormData" && e[Dr]);
}
const qc = (e) => {
  const t = new Array(10), r = (n, s) => {
    if (Wt(n)) {
      if (t.indexOf(n) >= 0)
        return;
      if (zt(n))
        return n;
      if (!("toJSON" in n)) {
        t[s] = n;
        const a = vt(n) ? [] : {};
        return Ht(n, (i, l) => {
          const u = r(i, s + 1);
          !xt(u) && (a[l] = u);
        }), t[s] = void 0, a;
      }
    }
    return n;
  };
  return r(e, 0);
}, Kc = Oe("AsyncFunction"), Jc = (e) => e && (Wt(e) || ge(e)) && ge(e.then) && ge(e.catch), oa = ((e, t) => e ? setImmediate : t ? ((r, n) => (Ge.addEventListener("message", ({ source: s, data: a }) => {
  s === Ge && a === r && n.length && n.shift()();
}, !1), (s) => {
  n.push(s), Ge.postMessage(r, "*");
}))(`axios@${Math.random()}`, []) : (r) => setTimeout(r))(
  typeof setImmediate == "function",
  ge(Ge.postMessage)
), Qc = typeof queueMicrotask < "u" ? queueMicrotask.bind(Ge) : typeof process < "u" && process.nextTick || oa, Gc = (e) => e != null && ge(e[Dr]), x = {
  isArray: vt,
  isArrayBuffer: ea,
  isBuffer: zt,
  isFormData: Ec,
  isArrayBufferView: hc,
  isString: pc,
  isNumber: ta,
  isBoolean: mc,
  isObject: Wt,
  isPlainObject: lr,
  isEmptyObject: gc,
  isReadableStream: Nc,
  isRequest: jc,
  isResponse: Sc,
  isHeaders: _c,
  isUndefined: xt,
  isDate: yc,
  isFile: xc,
  isBlob: bc,
  isRegExp: $c,
  isFunction: ge,
  isStream: wc,
  isURLSearchParams: kc,
  isTypedArray: Pc,
  isFileList: vc,
  forEach: Ht,
  merge: wn,
  extend: Oc,
  trim: Rc,
  stripBOM: Tc,
  inherits: Cc,
  toFlatObject: Ac,
  kindOf: Mr,
  kindOfTest: Oe,
  endsWith: Dc,
  toArray: Mc,
  forEachEntry: Lc,
  matchAll: Ic,
  isHTMLForm: Fc,
  hasOwnProperty: Cs,
  hasOwnProp: Cs,
  // an alias to avoid ESLint no-prototype-builtins detection
  reduceDescriptors: sa,
  freezeMethods: Bc,
  toObjectSet: Vc,
  toCamelCase: Uc,
  noop: zc,
  toFiniteNumber: Wc,
  findKey: ra,
  global: Ge,
  isContextDefined: na,
  isSpecCompliantForm: Hc,
  toJSONObject: qc,
  isAsyncFn: Kc,
  isThenable: Jc,
  setImmediate: oa,
  asap: Qc,
  isIterable: Gc
};
function F(e, t, r, n, s) {
  Error.call(this), Error.captureStackTrace ? Error.captureStackTrace(this, this.constructor) : this.stack = new Error().stack, this.message = e, this.name = "AxiosError", t && (this.code = t), r && (this.config = r), n && (this.request = n), s && (this.response = s, this.status = s.status ? s.status : null);
}
x.inherits(F, Error, {
  toJSON: function() {
    return {
      // Standard
      message: this.message,
      name: this.name,
      // Microsoft
      description: this.description,
      number: this.number,
      // Mozilla
      fileName: this.fileName,
      lineNumber: this.lineNumber,
      columnNumber: this.columnNumber,
      stack: this.stack,
      // Axios
      config: x.toJSONObject(this.config),
      code: this.code,
      status: this.status
    };
  }
});
const aa = F.prototype, ia = {};
[
  "ERR_BAD_OPTION_VALUE",
  "ERR_BAD_OPTION",
  "ECONNABORTED",
  "ETIMEDOUT",
  "ERR_NETWORK",
  "ERR_FR_TOO_MANY_REDIRECTS",
  "ERR_DEPRECATED",
  "ERR_BAD_RESPONSE",
  "ERR_BAD_REQUEST",
  "ERR_CANCELED",
  "ERR_NOT_SUPPORT",
  "ERR_INVALID_URL"
  // eslint-disable-next-line func-names
].forEach((e) => {
  ia[e] = { value: e };
});
Object.defineProperties(F, ia);
Object.defineProperty(aa, "isAxiosError", { value: !0 });
F.from = (e, t, r, n, s, a) => {
  const i = Object.create(aa);
  x.toFlatObject(e, i, function(c) {
    return c !== Error.prototype;
  }, (d) => d !== "isAxiosError");
  const l = e && e.message ? e.message : "Error", u = t == null && e ? e.code : t;
  return F.call(i, l, u, r, n, s), e && i.cause == null && Object.defineProperty(i, "cause", { value: e, configurable: !0 }), i.name = e && e.name || "Error", a && Object.assign(i, a), i;
};
const Yc = null;
function En(e) {
  return x.isPlainObject(e) || x.isArray(e);
}
function la(e) {
  return x.endsWith(e, "[]") ? e.slice(0, -2) : e;
}
function As(e, t, r) {
  return e ? e.concat(t).map(function(s, a) {
    return s = la(s), !r && a ? "[" + s + "]" : s;
  }).join(r ? "." : "") : t;
}
function Xc(e) {
  return x.isArray(e) && !e.some(En);
}
const Zc = x.toFlatObject(x, {}, null, function(t) {
  return /^is[A-Z]/.test(t);
});
function Lr(e, t, r) {
  if (!x.isObject(e))
    throw new TypeError("target must be an object");
  t = t || new FormData(), r = x.toFlatObject(r, {
    metaTokens: !0,
    dots: !1,
    indexes: !1
  }, !1, function(b, g) {
    return !x.isUndefined(g[b]);
  });
  const n = r.metaTokens, s = r.visitor || c, a = r.dots, i = r.indexes, u = (r.Blob || typeof Blob < "u" && Blob) && x.isSpecCompliantForm(t);
  if (!x.isFunction(s))
    throw new TypeError("visitor must be a function");
  function d(p) {
    if (p === null) return "";
    if (x.isDate(p))
      return p.toISOString();
    if (x.isBoolean(p))
      return p.toString();
    if (!u && x.isBlob(p))
      throw new F("Blob is not supported. Use a Buffer instead.");
    return x.isArrayBuffer(p) || x.isTypedArray(p) ? u && typeof Blob == "function" ? new Blob([p]) : Buffer.from(p) : p;
  }
  function c(p, b, g) {
    let E = p;
    if (p && !g && typeof p == "object") {
      if (x.endsWith(b, "{}"))
        b = n ? b : b.slice(0, -2), p = JSON.stringify(p);
      else if (x.isArray(p) && Xc(p) || (x.isFileList(p) || x.endsWith(b, "[]")) && (E = x.toArray(p)))
        return b = la(b), E.forEach(function(_, w) {
          !(x.isUndefined(_) || _ === null) && t.append(
            // eslint-disable-next-line no-nested-ternary
            i === !0 ? As([b], w, a) : i === null ? b : b + "[]",
            d(_)
          );
        }), !1;
    }
    return En(p) ? !0 : (t.append(As(g, b, a), d(p)), !1);
  }
  const f = [], y = Object.assign(Zc, {
    defaultVisitor: c,
    convertValue: d,
    isVisitable: En
  });
  function m(p, b) {
    if (!x.isUndefined(p)) {
      if (f.indexOf(p) !== -1)
        throw Error("Circular reference detected in " + b.join("."));
      f.push(p), x.forEach(p, function(E, N) {
        (!(x.isUndefined(E) || E === null) && s.call(
          t,
          E,
          x.isString(N) ? N.trim() : N,
          b,
          y
        )) === !0 && m(E, b ? b.concat(N) : [N]);
      }), f.pop();
    }
  }
  if (!x.isObject(e))
    throw new TypeError("data must be an object");
  return m(e), t;
}
function Ds(e) {
  const t = {
    "!": "%21",
    "'": "%27",
    "(": "%28",
    ")": "%29",
    "~": "%7E",
    "%20": "+",
    "%00": "\0"
  };
  return encodeURIComponent(e).replace(/[!'()~]|%20|%00/g, function(n) {
    return t[n];
  });
}
function Hn(e, t) {
  this._pairs = [], e && Lr(e, this, t);
}
const ca = Hn.prototype;
ca.append = function(t, r) {
  this._pairs.push([t, r]);
};
ca.toString = function(t) {
  const r = t ? function(n) {
    return t.call(this, n, Ds);
  } : Ds;
  return this._pairs.map(function(s) {
    return r(s[0]) + "=" + r(s[1]);
  }, "").join("&");
};
function ed(e) {
  return encodeURIComponent(e).replace(/%3A/gi, ":").replace(/%24/g, "$").replace(/%2C/gi, ",").replace(/%20/g, "+");
}
function da(e, t, r) {
  if (!t)
    return e;
  const n = r && r.encode || ed;
  x.isFunction(r) && (r = {
    serialize: r
  });
  const s = r && r.serialize;
  let a;
  if (s ? a = s(t, r) : a = x.isURLSearchParams(t) ? t.toString() : new Hn(t, r).toString(n), a) {
    const i = e.indexOf("#");
    i !== -1 && (e = e.slice(0, i)), e += (e.indexOf("?") === -1 ? "?" : "&") + a;
  }
  return e;
}
class Ms {
  constructor() {
    this.handlers = [];
  }
  /**
   * Add a new interceptor to the stack
   *
   * @param {Function} fulfilled The function to handle `then` for a `Promise`
   * @param {Function} rejected The function to handle `reject` for a `Promise`
   *
   * @return {Number} An ID used to remove interceptor later
   */
  use(t, r, n) {
    return this.handlers.push({
      fulfilled: t,
      rejected: r,
      synchronous: n ? n.synchronous : !1,
      runWhen: n ? n.runWhen : null
    }), this.handlers.length - 1;
  }
  /**
   * Remove an interceptor from the stack
   *
   * @param {Number} id The ID that was returned by `use`
   *
   * @returns {void}
   */
  eject(t) {
    this.handlers[t] && (this.handlers[t] = null);
  }
  /**
   * Clear all interceptors from the stack
   *
   * @returns {void}
   */
  clear() {
    this.handlers && (this.handlers = []);
  }
  /**
   * Iterate over all the registered interceptors
   *
   * This method is particularly useful for skipping over any
   * interceptors that may have become `null` calling `eject`.
   *
   * @param {Function} fn The function to call for each interceptor
   *
   * @returns {void}
   */
  forEach(t) {
    x.forEach(this.handlers, function(n) {
      n !== null && t(n);
    });
  }
}
const ua = {
  silentJSONParsing: !0,
  forcedJSONParsing: !0,
  clarifyTimeoutError: !1
}, td = typeof URLSearchParams < "u" ? URLSearchParams : Hn, rd = typeof FormData < "u" ? FormData : null, nd = typeof Blob < "u" ? Blob : null, sd = {
  isBrowser: !0,
  classes: {
    URLSearchParams: td,
    FormData: rd,
    Blob: nd
  },
  protocols: ["http", "https", "file", "blob", "url", "data"]
}, qn = typeof window < "u" && typeof document < "u", kn = typeof navigator == "object" && navigator || void 0, od = qn && (!kn || ["ReactNative", "NativeScript", "NS"].indexOf(kn.product) < 0), ad = typeof WorkerGlobalScope < "u" && // eslint-disable-next-line no-undef
self instanceof WorkerGlobalScope && typeof self.importScripts == "function", id = qn && window.location.href || "http://localhost", ld = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  hasBrowserEnv: qn,
  hasStandardBrowserEnv: od,
  hasStandardBrowserWebWorkerEnv: ad,
  navigator: kn,
  origin: id
}, Symbol.toStringTag, { value: "Module" })), ie = {
  ...ld,
  ...sd
};
function cd(e, t) {
  return Lr(e, new ie.classes.URLSearchParams(), {
    visitor: function(r, n, s, a) {
      return ie.isNode && x.isBuffer(r) ? (this.append(n, r.toString("base64")), !1) : a.defaultVisitor.apply(this, arguments);
    },
    ...t
  });
}
function dd(e) {
  return x.matchAll(/\w+|\[(\w*)]/g, e).map((t) => t[0] === "[]" ? "" : t[1] || t[0]);
}
function ud(e) {
  const t = {}, r = Object.keys(e);
  let n;
  const s = r.length;
  let a;
  for (n = 0; n < s; n++)
    a = r[n], t[a] = e[a];
  return t;
}
function fa(e) {
  function t(r, n, s, a) {
    let i = r[a++];
    if (i === "__proto__") return !0;
    const l = Number.isFinite(+i), u = a >= r.length;
    return i = !i && x.isArray(s) ? s.length : i, u ? (x.hasOwnProp(s, i) ? s[i] = [s[i], n] : s[i] = n, !l) : ((!s[i] || !x.isObject(s[i])) && (s[i] = []), t(r, n, s[i], a) && x.isArray(s[i]) && (s[i] = ud(s[i])), !l);
  }
  if (x.isFormData(e) && x.isFunction(e.entries)) {
    const r = {};
    return x.forEachEntry(e, (n, s) => {
      t(dd(n), s, r, 0);
    }), r;
  }
  return null;
}
function fd(e, t, r) {
  if (x.isString(e))
    try {
      return (t || JSON.parse)(e), x.trim(e);
    } catch (n) {
      if (n.name !== "SyntaxError")
        throw n;
    }
  return (r || JSON.stringify)(e);
}
const qt = {
  transitional: ua,
  adapter: ["xhr", "http", "fetch"],
  transformRequest: [function(t, r) {
    const n = r.getContentType() || "", s = n.indexOf("application/json") > -1, a = x.isObject(t);
    if (a && x.isHTMLForm(t) && (t = new FormData(t)), x.isFormData(t))
      return s ? JSON.stringify(fa(t)) : t;
    if (x.isArrayBuffer(t) || x.isBuffer(t) || x.isStream(t) || x.isFile(t) || x.isBlob(t) || x.isReadableStream(t))
      return t;
    if (x.isArrayBufferView(t))
      return t.buffer;
    if (x.isURLSearchParams(t))
      return r.setContentType("application/x-www-form-urlencoded;charset=utf-8", !1), t.toString();
    let l;
    if (a) {
      if (n.indexOf("application/x-www-form-urlencoded") > -1)
        return cd(t, this.formSerializer).toString();
      if ((l = x.isFileList(t)) || n.indexOf("multipart/form-data") > -1) {
        const u = this.env && this.env.FormData;
        return Lr(
          l ? { "files[]": t } : t,
          u && new u(),
          this.formSerializer
        );
      }
    }
    return a || s ? (r.setContentType("application/json", !1), fd(t)) : t;
  }],
  transformResponse: [function(t) {
    const r = this.transitional || qt.transitional, n = r && r.forcedJSONParsing, s = this.responseType === "json";
    if (x.isResponse(t) || x.isReadableStream(t))
      return t;
    if (t && x.isString(t) && (n && !this.responseType || s)) {
      const i = !(r && r.silentJSONParsing) && s;
      try {
        return JSON.parse(t, this.parseReviver);
      } catch (l) {
        if (i)
          throw l.name === "SyntaxError" ? F.from(l, F.ERR_BAD_RESPONSE, this, null, this.response) : l;
      }
    }
    return t;
  }],
  /**
   * A timeout in milliseconds to abort a request. If set to 0 (default) a
   * timeout is not created.
   */
  timeout: 0,
  xsrfCookieName: "XSRF-TOKEN",
  xsrfHeaderName: "X-XSRF-TOKEN",
  maxContentLength: -1,
  maxBodyLength: -1,
  env: {
    FormData: ie.classes.FormData,
    Blob: ie.classes.Blob
  },
  validateStatus: function(t) {
    return t >= 200 && t < 300;
  },
  headers: {
    common: {
      Accept: "application/json, text/plain, */*",
      "Content-Type": void 0
    }
  }
};
x.forEach(["delete", "get", "head", "post", "put", "patch"], (e) => {
  qt.headers[e] = {};
});
const hd = x.toObjectSet([
  "age",
  "authorization",
  "content-length",
  "content-type",
  "etag",
  "expires",
  "from",
  "host",
  "if-modified-since",
  "if-unmodified-since",
  "last-modified",
  "location",
  "max-forwards",
  "proxy-authorization",
  "referer",
  "retry-after",
  "user-agent"
]), pd = (e) => {
  const t = {};
  let r, n, s;
  return e && e.split(`
`).forEach(function(i) {
    s = i.indexOf(":"), r = i.substring(0, s).trim().toLowerCase(), n = i.substring(s + 1).trim(), !(!r || t[r] && hd[r]) && (r === "set-cookie" ? t[r] ? t[r].push(n) : t[r] = [n] : t[r] = t[r] ? t[r] + ", " + n : n);
  }), t;
}, Ps = Symbol("internals");
function _t(e) {
  return e && String(e).trim().toLowerCase();
}
function cr(e) {
  return e === !1 || e == null ? e : x.isArray(e) ? e.map(cr) : String(e);
}
function md(e) {
  const t = /* @__PURE__ */ Object.create(null), r = /([^\s,;=]+)\s*(?:=\s*([^,;]+))?/g;
  let n;
  for (; n = r.exec(e); )
    t[n[1]] = n[2];
  return t;
}
const gd = (e) => /^[-_a-zA-Z0-9^`|~,!#$%&'*+.]+$/.test(e.trim());
function on(e, t, r, n, s) {
  if (x.isFunction(n))
    return n.call(this, t, r);
  if (s && (t = r), !!x.isString(t)) {
    if (x.isString(n))
      return t.indexOf(n) !== -1;
    if (x.isRegExp(n))
      return n.test(t);
  }
}
function yd(e) {
  return e.trim().toLowerCase().replace(/([a-z\d])(\w*)/g, (t, r, n) => r.toUpperCase() + n);
}
function xd(e, t) {
  const r = x.toCamelCase(" " + t);
  ["get", "set", "has"].forEach((n) => {
    Object.defineProperty(e, n + r, {
      value: function(s, a, i) {
        return this[n].call(this, t, s, a, i);
      },
      configurable: !0
    });
  });
}
let ye = class {
  constructor(t) {
    t && this.set(t);
  }
  set(t, r, n) {
    const s = this;
    function a(l, u, d) {
      const c = _t(u);
      if (!c)
        throw new Error("header name must be a non-empty string");
      const f = x.findKey(s, c);
      (!f || s[f] === void 0 || d === !0 || d === void 0 && s[f] !== !1) && (s[f || u] = cr(l));
    }
    const i = (l, u) => x.forEach(l, (d, c) => a(d, c, u));
    if (x.isPlainObject(t) || t instanceof this.constructor)
      i(t, r);
    else if (x.isString(t) && (t = t.trim()) && !gd(t))
      i(pd(t), r);
    else if (x.isObject(t) && x.isIterable(t)) {
      let l = {}, u, d;
      for (const c of t) {
        if (!x.isArray(c))
          throw TypeError("Object iterator must return a key-value pair");
        l[d = c[0]] = (u = l[d]) ? x.isArray(u) ? [...u, c[1]] : [u, c[1]] : c[1];
      }
      i(l, r);
    } else
      t != null && a(r, t, n);
    return this;
  }
  get(t, r) {
    if (t = _t(t), t) {
      const n = x.findKey(this, t);
      if (n) {
        const s = this[n];
        if (!r)
          return s;
        if (r === !0)
          return md(s);
        if (x.isFunction(r))
          return r.call(this, s, n);
        if (x.isRegExp(r))
          return r.exec(s);
        throw new TypeError("parser must be boolean|regexp|function");
      }
    }
  }
  has(t, r) {
    if (t = _t(t), t) {
      const n = x.findKey(this, t);
      return !!(n && this[n] !== void 0 && (!r || on(this, this[n], n, r)));
    }
    return !1;
  }
  delete(t, r) {
    const n = this;
    let s = !1;
    function a(i) {
      if (i = _t(i), i) {
        const l = x.findKey(n, i);
        l && (!r || on(n, n[l], l, r)) && (delete n[l], s = !0);
      }
    }
    return x.isArray(t) ? t.forEach(a) : a(t), s;
  }
  clear(t) {
    const r = Object.keys(this);
    let n = r.length, s = !1;
    for (; n--; ) {
      const a = r[n];
      (!t || on(this, this[a], a, t, !0)) && (delete this[a], s = !0);
    }
    return s;
  }
  normalize(t) {
    const r = this, n = {};
    return x.forEach(this, (s, a) => {
      const i = x.findKey(n, a);
      if (i) {
        r[i] = cr(s), delete r[a];
        return;
      }
      const l = t ? yd(a) : String(a).trim();
      l !== a && delete r[a], r[l] = cr(s), n[l] = !0;
    }), this;
  }
  concat(...t) {
    return this.constructor.concat(this, ...t);
  }
  toJSON(t) {
    const r = /* @__PURE__ */ Object.create(null);
    return x.forEach(this, (n, s) => {
      n != null && n !== !1 && (r[s] = t && x.isArray(n) ? n.join(", ") : n);
    }), r;
  }
  [Symbol.iterator]() {
    return Object.entries(this.toJSON())[Symbol.iterator]();
  }
  toString() {
    return Object.entries(this.toJSON()).map(([t, r]) => t + ": " + r).join(`
`);
  }
  getSetCookie() {
    return this.get("set-cookie") || [];
  }
  get [Symbol.toStringTag]() {
    return "AxiosHeaders";
  }
  static from(t) {
    return t instanceof this ? t : new this(t);
  }
  static concat(t, ...r) {
    const n = new this(t);
    return r.forEach((s) => n.set(s)), n;
  }
  static accessor(t) {
    const n = (this[Ps] = this[Ps] = {
      accessors: {}
    }).accessors, s = this.prototype;
    function a(i) {
      const l = _t(i);
      n[l] || (xd(s, i), n[l] = !0);
    }
    return x.isArray(t) ? t.forEach(a) : a(t), this;
  }
};
ye.accessor(["Content-Type", "Content-Length", "Accept", "Accept-Encoding", "User-Agent", "Authorization"]);
x.reduceDescriptors(ye.prototype, ({ value: e }, t) => {
  let r = t[0].toUpperCase() + t.slice(1);
  return {
    get: () => e,
    set(n) {
      this[r] = n;
    }
  };
});
x.freezeMethods(ye);
function an(e, t) {
  const r = this || qt, n = t || r, s = ye.from(n.headers);
  let a = n.data;
  return x.forEach(e, function(l) {
    a = l.call(r, a, s.normalize(), t ? t.status : void 0);
  }), s.normalize(), a;
}
function ha(e) {
  return !!(e && e.__CANCEL__);
}
function wt(e, t, r) {
  F.call(this, e ?? "canceled", F.ERR_CANCELED, t, r), this.name = "CanceledError";
}
x.inherits(wt, F, {
  __CANCEL__: !0
});
function pa(e, t, r) {
  const n = r.config.validateStatus;
  !r.status || !n || n(r.status) ? e(r) : t(new F(
    "Request failed with status code " + r.status,
    [F.ERR_BAD_REQUEST, F.ERR_BAD_RESPONSE][Math.floor(r.status / 100) - 4],
    r.config,
    r.request,
    r
  ));
}
function bd(e) {
  const t = /^([-+\w]{1,25})(:?\/\/|:)/.exec(e);
  return t && t[1] || "";
}
function vd(e, t) {
  e = e || 10;
  const r = new Array(e), n = new Array(e);
  let s = 0, a = 0, i;
  return t = t !== void 0 ? t : 1e3, function(u) {
    const d = Date.now(), c = n[a];
    i || (i = d), r[s] = u, n[s] = d;
    let f = a, y = 0;
    for (; f !== s; )
      y += r[f++], f = f % e;
    if (s = (s + 1) % e, s === a && (a = (a + 1) % e), d - i < t)
      return;
    const m = c && d - c;
    return m ? Math.round(y * 1e3 / m) : void 0;
  };
}
function wd(e, t) {
  let r = 0, n = 1e3 / t, s, a;
  const i = (d, c = Date.now()) => {
    r = c, s = null, a && (clearTimeout(a), a = null), e(...d);
  };
  return [(...d) => {
    const c = Date.now(), f = c - r;
    f >= n ? i(d, c) : (s = d, a || (a = setTimeout(() => {
      a = null, i(s);
    }, n - f)));
  }, () => s && i(s)];
}
const wr = (e, t, r = 3) => {
  let n = 0;
  const s = vd(50, 250);
  return wd((a) => {
    const i = a.loaded, l = a.lengthComputable ? a.total : void 0, u = i - n, d = s(u), c = i <= l;
    n = i;
    const f = {
      loaded: i,
      total: l,
      progress: l ? i / l : void 0,
      bytes: u,
      rate: d || void 0,
      estimated: d && l && c ? (l - i) / d : void 0,
      event: a,
      lengthComputable: l != null,
      [t ? "download" : "upload"]: !0
    };
    e(f);
  }, r);
}, Ls = (e, t) => {
  const r = e != null;
  return [(n) => t[0]({
    lengthComputable: r,
    total: e,
    loaded: n
  }), t[1]];
}, Is = (e) => (...t) => x.asap(() => e(...t)), Ed = ie.hasStandardBrowserEnv ? /* @__PURE__ */ ((e, t) => (r) => (r = new URL(r, ie.origin), e.protocol === r.protocol && e.host === r.host && (t || e.port === r.port)))(
  new URL(ie.origin),
  ie.navigator && /(msie|trident)/i.test(ie.navigator.userAgent)
) : () => !0, kd = ie.hasStandardBrowserEnv ? (
  // Standard browser envs support document.cookie
  {
    write(e, t, r, n, s, a, i) {
      if (typeof document > "u") return;
      const l = [`${e}=${encodeURIComponent(t)}`];
      x.isNumber(r) && l.push(`expires=${new Date(r).toUTCString()}`), x.isString(n) && l.push(`path=${n}`), x.isString(s) && l.push(`domain=${s}`), a === !0 && l.push("secure"), x.isString(i) && l.push(`SameSite=${i}`), document.cookie = l.join("; ");
    },
    read(e) {
      if (typeof document > "u") return null;
      const t = document.cookie.match(new RegExp("(?:^|; )" + e + "=([^;]*)"));
      return t ? decodeURIComponent(t[1]) : null;
    },
    remove(e) {
      this.write(e, "", Date.now() - 864e5, "/");
    }
  }
) : (
  // Non-standard browser env (web workers, react-native) lack needed support.
  {
    write() {
    },
    read() {
      return null;
    },
    remove() {
    }
  }
);
function Nd(e) {
  return /^([a-z][a-z\d+\-.]*:)?\/\//i.test(e);
}
function jd(e, t) {
  return t ? e.replace(/\/?\/$/, "") + "/" + t.replace(/^\/+/, "") : e;
}
function ma(e, t, r) {
  let n = !Nd(t);
  return e && (n || r == !1) ? jd(e, t) : t;
}
const Fs = (e) => e instanceof ye ? { ...e } : e;
function ot(e, t) {
  t = t || {};
  const r = {};
  function n(d, c, f, y) {
    return x.isPlainObject(d) && x.isPlainObject(c) ? x.merge.call({ caseless: y }, d, c) : x.isPlainObject(c) ? x.merge({}, c) : x.isArray(c) ? c.slice() : c;
  }
  function s(d, c, f, y) {
    if (x.isUndefined(c)) {
      if (!x.isUndefined(d))
        return n(void 0, d, f, y);
    } else return n(d, c, f, y);
  }
  function a(d, c) {
    if (!x.isUndefined(c))
      return n(void 0, c);
  }
  function i(d, c) {
    if (x.isUndefined(c)) {
      if (!x.isUndefined(d))
        return n(void 0, d);
    } else return n(void 0, c);
  }
  function l(d, c, f) {
    if (f in t)
      return n(d, c);
    if (f in e)
      return n(void 0, d);
  }
  const u = {
    url: a,
    method: a,
    data: a,
    baseURL: i,
    transformRequest: i,
    transformResponse: i,
    paramsSerializer: i,
    timeout: i,
    timeoutMessage: i,
    withCredentials: i,
    withXSRFToken: i,
    adapter: i,
    responseType: i,
    xsrfCookieName: i,
    xsrfHeaderName: i,
    onUploadProgress: i,
    onDownloadProgress: i,
    decompress: i,
    maxContentLength: i,
    maxBodyLength: i,
    beforeRedirect: i,
    transport: i,
    httpAgent: i,
    httpsAgent: i,
    cancelToken: i,
    socketPath: i,
    responseEncoding: i,
    validateStatus: l,
    headers: (d, c, f) => s(Fs(d), Fs(c), f, !0)
  };
  return x.forEach(Object.keys({ ...e, ...t }), function(c) {
    const f = u[c] || s, y = f(e[c], t[c], c);
    x.isUndefined(y) && f !== l || (r[c] = y);
  }), r;
}
const ga = (e) => {
  const t = ot({}, e);
  let { data: r, withXSRFToken: n, xsrfHeaderName: s, xsrfCookieName: a, headers: i, auth: l } = t;
  if (t.headers = i = ye.from(i), t.url = da(ma(t.baseURL, t.url, t.allowAbsoluteUrls), e.params, e.paramsSerializer), l && i.set(
    "Authorization",
    "Basic " + btoa((l.username || "") + ":" + (l.password ? unescape(encodeURIComponent(l.password)) : ""))
  ), x.isFormData(r)) {
    if (ie.hasStandardBrowserEnv || ie.hasStandardBrowserWebWorkerEnv)
      i.setContentType(void 0);
    else if (x.isFunction(r.getHeaders)) {
      const u = r.getHeaders(), d = ["content-type", "content-length"];
      Object.entries(u).forEach(([c, f]) => {
        d.includes(c.toLowerCase()) && i.set(c, f);
      });
    }
  }
  if (ie.hasStandardBrowserEnv && (n && x.isFunction(n) && (n = n(t)), n || n !== !1 && Ed(t.url))) {
    const u = s && a && kd.read(a);
    u && i.set(s, u);
  }
  return t;
}, Sd = typeof XMLHttpRequest < "u", _d = Sd && function(e) {
  return new Promise(function(r, n) {
    const s = ga(e);
    let a = s.data;
    const i = ye.from(s.headers).normalize();
    let { responseType: l, onUploadProgress: u, onDownloadProgress: d } = s, c, f, y, m, p;
    function b() {
      m && m(), p && p(), s.cancelToken && s.cancelToken.unsubscribe(c), s.signal && s.signal.removeEventListener("abort", c);
    }
    let g = new XMLHttpRequest();
    g.open(s.method.toUpperCase(), s.url, !0), g.timeout = s.timeout;
    function E() {
      if (!g)
        return;
      const _ = ye.from(
        "getAllResponseHeaders" in g && g.getAllResponseHeaders()
      ), S = {
        data: !l || l === "text" || l === "json" ? g.responseText : g.response,
        status: g.status,
        statusText: g.statusText,
        headers: _,
        config: e,
        request: g
      };
      pa(function(D) {
        r(D), b();
      }, function(D) {
        n(D), b();
      }, S), g = null;
    }
    "onloadend" in g ? g.onloadend = E : g.onreadystatechange = function() {
      !g || g.readyState !== 4 || g.status === 0 && !(g.responseURL && g.responseURL.indexOf("file:") === 0) || setTimeout(E);
    }, g.onabort = function() {
      g && (n(new F("Request aborted", F.ECONNABORTED, e, g)), g = null);
    }, g.onerror = function(w) {
      const S = w && w.message ? w.message : "Network Error", A = new F(S, F.ERR_NETWORK, e, g);
      A.event = w || null, n(A), g = null;
    }, g.ontimeout = function() {
      let w = s.timeout ? "timeout of " + s.timeout + "ms exceeded" : "timeout exceeded";
      const S = s.transitional || ua;
      s.timeoutErrorMessage && (w = s.timeoutErrorMessage), n(new F(
        w,
        S.clarifyTimeoutError ? F.ETIMEDOUT : F.ECONNABORTED,
        e,
        g
      )), g = null;
    }, a === void 0 && i.setContentType(null), "setRequestHeader" in g && x.forEach(i.toJSON(), function(w, S) {
      g.setRequestHeader(S, w);
    }), x.isUndefined(s.withCredentials) || (g.withCredentials = !!s.withCredentials), l && l !== "json" && (g.responseType = s.responseType), d && ([y, p] = wr(d, !0), g.addEventListener("progress", y)), u && g.upload && ([f, m] = wr(u), g.upload.addEventListener("progress", f), g.upload.addEventListener("loadend", m)), (s.cancelToken || s.signal) && (c = (_) => {
      g && (n(!_ || _.type ? new wt(null, e, g) : _), g.abort(), g = null);
    }, s.cancelToken && s.cancelToken.subscribe(c), s.signal && (s.signal.aborted ? c() : s.signal.addEventListener("abort", c)));
    const N = bd(s.url);
    if (N && ie.protocols.indexOf(N) === -1) {
      n(new F("Unsupported protocol " + N + ":", F.ERR_BAD_REQUEST, e));
      return;
    }
    g.send(a || null);
  });
}, Rd = (e, t) => {
  const { length: r } = e = e ? e.filter(Boolean) : [];
  if (t || r) {
    let n = new AbortController(), s;
    const a = function(d) {
      if (!s) {
        s = !0, l();
        const c = d instanceof Error ? d : this.reason;
        n.abort(c instanceof F ? c : new wt(c instanceof Error ? c.message : c));
      }
    };
    let i = t && setTimeout(() => {
      i = null, a(new F(`timeout ${t} of ms exceeded`, F.ETIMEDOUT));
    }, t);
    const l = () => {
      e && (i && clearTimeout(i), i = null, e.forEach((d) => {
        d.unsubscribe ? d.unsubscribe(a) : d.removeEventListener("abort", a);
      }), e = null);
    };
    e.forEach((d) => d.addEventListener("abort", a));
    const { signal: u } = n;
    return u.unsubscribe = () => x.asap(l), u;
  }
}, Od = function* (e, t) {
  let r = e.byteLength;
  if (r < t) {
    yield e;
    return;
  }
  let n = 0, s;
  for (; n < r; )
    s = n + t, yield e.slice(n, s), n = s;
}, Td = async function* (e, t) {
  for await (const r of Cd(e))
    yield* Od(r, t);
}, Cd = async function* (e) {
  if (e[Symbol.asyncIterator]) {
    yield* e;
    return;
  }
  const t = e.getReader();
  try {
    for (; ; ) {
      const { done: r, value: n } = await t.read();
      if (r)
        break;
      yield n;
    }
  } finally {
    await t.cancel();
  }
}, Us = (e, t, r, n) => {
  const s = Td(e, t);
  let a = 0, i, l = (u) => {
    i || (i = !0, n && n(u));
  };
  return new ReadableStream({
    async pull(u) {
      try {
        const { done: d, value: c } = await s.next();
        if (d) {
          l(), u.close();
          return;
        }
        let f = c.byteLength;
        if (r) {
          let y = a += f;
          r(y);
        }
        u.enqueue(new Uint8Array(c));
      } catch (d) {
        throw l(d), d;
      }
    },
    cancel(u) {
      return l(u), s.return();
    }
  }, {
    highWaterMark: 2
  });
}, $s = 64 * 1024, { isFunction: nr } = x, Ad = (({ Request: e, Response: t }) => ({
  Request: e,
  Response: t
}))(x.global), {
  ReadableStream: Bs,
  TextEncoder: Vs
} = x.global, zs = (e, ...t) => {
  try {
    return !!e(...t);
  } catch {
    return !1;
  }
}, Dd = (e) => {
  e = x.merge.call({
    skipUndefined: !0
  }, Ad, e);
  const { fetch: t, Request: r, Response: n } = e, s = t ? nr(t) : typeof fetch == "function", a = nr(r), i = nr(n);
  if (!s)
    return !1;
  const l = s && nr(Bs), u = s && (typeof Vs == "function" ? /* @__PURE__ */ ((p) => (b) => p.encode(b))(new Vs()) : async (p) => new Uint8Array(await new r(p).arrayBuffer())), d = a && l && zs(() => {
    let p = !1;
    const b = new r(ie.origin, {
      body: new Bs(),
      method: "POST",
      get duplex() {
        return p = !0, "half";
      }
    }).headers.has("Content-Type");
    return p && !b;
  }), c = i && l && zs(() => x.isReadableStream(new n("").body)), f = {
    stream: c && ((p) => p.body)
  };
  s && ["text", "arrayBuffer", "blob", "formData", "stream"].forEach((p) => {
    !f[p] && (f[p] = (b, g) => {
      let E = b && b[p];
      if (E)
        return E.call(b);
      throw new F(`Response type '${p}' is not supported`, F.ERR_NOT_SUPPORT, g);
    });
  });
  const y = async (p) => {
    if (p == null)
      return 0;
    if (x.isBlob(p))
      return p.size;
    if (x.isSpecCompliantForm(p))
      return (await new r(ie.origin, {
        method: "POST",
        body: p
      }).arrayBuffer()).byteLength;
    if (x.isArrayBufferView(p) || x.isArrayBuffer(p))
      return p.byteLength;
    if (x.isURLSearchParams(p) && (p = p + ""), x.isString(p))
      return (await u(p)).byteLength;
  }, m = async (p, b) => {
    const g = x.toFiniteNumber(p.getContentLength());
    return g ?? y(b);
  };
  return async (p) => {
    let {
      url: b,
      method: g,
      data: E,
      signal: N,
      cancelToken: _,
      timeout: w,
      onDownloadProgress: S,
      onUploadProgress: A,
      responseType: D,
      headers: U,
      withCredentials: T = "same-origin",
      fetchOptions: M
    } = ga(p), C = t || fetch;
    D = D ? (D + "").toLowerCase() : "text";
    let $ = Rd([N, _ && _.toAbortSignal()], w), R = null;
    const V = $ && $.unsubscribe && (() => {
      $.unsubscribe();
    });
    let L;
    try {
      if (A && d && g !== "get" && g !== "head" && (L = await m(U, E)) !== 0) {
        let Ne = new r(b, {
          method: "POST",
          body: E,
          duplex: "half"
        }), De;
        if (x.isFormData(E) && (De = Ne.headers.get("content-type")) && U.setContentType(De), Ne.body) {
          const [at, it] = Ls(
            L,
            wr(Is(A))
          );
          E = Us(Ne.body, $s, at, it);
        }
      }
      x.isString(T) || (T = T ? "include" : "omit");
      const Y = a && "credentials" in r.prototype, Te = {
        ...M,
        signal: $,
        method: g.toUpperCase(),
        headers: U.normalize().toJSON(),
        body: E,
        duplex: "half",
        credentials: Y ? T : void 0
      };
      R = a && new r(b, Te);
      let ue = await (a ? C(R, M) : C(b, Te));
      const Et = c && (D === "stream" || D === "response");
      if (c && (S || Et && V)) {
        const Ne = {};
        ["status", "statusText", "headers"].forEach((Jt) => {
          Ne[Jt] = ue[Jt];
        });
        const De = x.toFiniteNumber(ue.headers.get("content-length")), [at, it] = S && Ls(
          De,
          wr(Is(S), !0)
        ) || [];
        ue = new n(
          Us(ue.body, $s, at, () => {
            it && it(), V && V();
          }),
          Ne
        );
      }
      D = D || "text";
      let Kt = await f[x.findKey(f, D) || "text"](ue, p);
      return !Et && V && V(), await new Promise((Ne, De) => {
        pa(Ne, De, {
          data: Kt,
          headers: ye.from(ue.headers),
          status: ue.status,
          statusText: ue.statusText,
          config: p,
          request: R
        });
      });
    } catch (Y) {
      throw V && V(), Y && Y.name === "TypeError" && /Load failed|fetch/i.test(Y.message) ? Object.assign(
        new F("Network Error", F.ERR_NETWORK, p, R),
        {
          cause: Y.cause || Y
        }
      ) : F.from(Y, Y && Y.code, p, R);
    }
  };
}, Md = /* @__PURE__ */ new Map(), ya = (e) => {
  let t = e && e.env || {};
  const { fetch: r, Request: n, Response: s } = t, a = [
    n,
    s,
    r
  ];
  let i = a.length, l = i, u, d, c = Md;
  for (; l--; )
    u = a[l], d = c.get(u), d === void 0 && c.set(u, d = l ? /* @__PURE__ */ new Map() : Dd(t)), c = d;
  return d;
};
ya();
const Kn = {
  http: Yc,
  xhr: _d,
  fetch: {
    get: ya
  }
};
x.forEach(Kn, (e, t) => {
  if (e) {
    try {
      Object.defineProperty(e, "name", { value: t });
    } catch {
    }
    Object.defineProperty(e, "adapterName", { value: t });
  }
});
const Ws = (e) => `- ${e}`, Pd = (e) => x.isFunction(e) || e === null || e === !1;
function Ld(e, t) {
  e = x.isArray(e) ? e : [e];
  const { length: r } = e;
  let n, s;
  const a = {};
  for (let i = 0; i < r; i++) {
    n = e[i];
    let l;
    if (s = n, !Pd(n) && (s = Kn[(l = String(n)).toLowerCase()], s === void 0))
      throw new F(`Unknown adapter '${l}'`);
    if (s && (x.isFunction(s) || (s = s.get(t))))
      break;
    a[l || "#" + i] = s;
  }
  if (!s) {
    const i = Object.entries(a).map(
      ([u, d]) => `adapter ${u} ` + (d === !1 ? "is not supported by the environment" : "is not available in the build")
    );
    let l = r ? i.length > 1 ? `since :
` + i.map(Ws).join(`
`) : " " + Ws(i[0]) : "as no adapter specified";
    throw new F(
      "There is no suitable adapter to dispatch the request " + l,
      "ERR_NOT_SUPPORT"
    );
  }
  return s;
}
const xa = {
  /**
   * Resolve an adapter from a list of adapter names or functions.
   * @type {Function}
   */
  getAdapter: Ld,
  /**
   * Exposes all known adapters
   * @type {Object<string, Function|Object>}
   */
  adapters: Kn
};
function ln(e) {
  if (e.cancelToken && e.cancelToken.throwIfRequested(), e.signal && e.signal.aborted)
    throw new wt(null, e);
}
function Hs(e) {
  return ln(e), e.headers = ye.from(e.headers), e.data = an.call(
    e,
    e.transformRequest
  ), ["post", "put", "patch"].indexOf(e.method) !== -1 && e.headers.setContentType("application/x-www-form-urlencoded", !1), xa.getAdapter(e.adapter || qt.adapter, e)(e).then(function(n) {
    return ln(e), n.data = an.call(
      e,
      e.transformResponse,
      n
    ), n.headers = ye.from(n.headers), n;
  }, function(n) {
    return ha(n) || (ln(e), n && n.response && (n.response.data = an.call(
      e,
      e.transformResponse,
      n.response
    ), n.response.headers = ye.from(n.response.headers))), Promise.reject(n);
  });
}
const ba = "1.13.2", Ir = {};
["object", "boolean", "number", "function", "string", "symbol"].forEach((e, t) => {
  Ir[e] = function(n) {
    return typeof n === e || "a" + (t < 1 ? "n " : " ") + e;
  };
});
const qs = {};
Ir.transitional = function(t, r, n) {
  function s(a, i) {
    return "[Axios v" + ba + "] Transitional option '" + a + "'" + i + (n ? ". " + n : "");
  }
  return (a, i, l) => {
    if (t === !1)
      throw new F(
        s(i, " has been removed" + (r ? " in " + r : "")),
        F.ERR_DEPRECATED
      );
    return r && !qs[i] && (qs[i] = !0, console.warn(
      s(
        i,
        " has been deprecated since v" + r + " and will be removed in the near future"
      )
    )), t ? t(a, i, l) : !0;
  };
};
Ir.spelling = function(t) {
  return (r, n) => (console.warn(`${n} is likely a misspelling of ${t}`), !0);
};
function Id(e, t, r) {
  if (typeof e != "object")
    throw new F("options must be an object", F.ERR_BAD_OPTION_VALUE);
  const n = Object.keys(e);
  let s = n.length;
  for (; s-- > 0; ) {
    const a = n[s], i = t[a];
    if (i) {
      const l = e[a], u = l === void 0 || i(l, a, e);
      if (u !== !0)
        throw new F("option " + a + " must be " + u, F.ERR_BAD_OPTION_VALUE);
      continue;
    }
    if (r !== !0)
      throw new F("Unknown option " + a, F.ERR_BAD_OPTION);
  }
}
const dr = {
  assertOptions: Id,
  validators: Ir
}, Ce = dr.validators;
let nt = class {
  constructor(t) {
    this.defaults = t || {}, this.interceptors = {
      request: new Ms(),
      response: new Ms()
    };
  }
  /**
   * Dispatch a request
   *
   * @param {String|Object} configOrUrl The config specific for this request (merged with this.defaults)
   * @param {?Object} config
   *
   * @returns {Promise} The Promise to be fulfilled
   */
  async request(t, r) {
    try {
      return await this._request(t, r);
    } catch (n) {
      if (n instanceof Error) {
        let s = {};
        Error.captureStackTrace ? Error.captureStackTrace(s) : s = new Error();
        const a = s.stack ? s.stack.replace(/^.+\n/, "") : "";
        try {
          n.stack ? a && !String(n.stack).endsWith(a.replace(/^.+\n.+\n/, "")) && (n.stack += `
` + a) : n.stack = a;
        } catch {
        }
      }
      throw n;
    }
  }
  _request(t, r) {
    typeof t == "string" ? (r = r || {}, r.url = t) : r = t || {}, r = ot(this.defaults, r);
    const { transitional: n, paramsSerializer: s, headers: a } = r;
    n !== void 0 && dr.assertOptions(n, {
      silentJSONParsing: Ce.transitional(Ce.boolean),
      forcedJSONParsing: Ce.transitional(Ce.boolean),
      clarifyTimeoutError: Ce.transitional(Ce.boolean)
    }, !1), s != null && (x.isFunction(s) ? r.paramsSerializer = {
      serialize: s
    } : dr.assertOptions(s, {
      encode: Ce.function,
      serialize: Ce.function
    }, !0)), r.allowAbsoluteUrls !== void 0 || (this.defaults.allowAbsoluteUrls !== void 0 ? r.allowAbsoluteUrls = this.defaults.allowAbsoluteUrls : r.allowAbsoluteUrls = !0), dr.assertOptions(r, {
      baseUrl: Ce.spelling("baseURL"),
      withXsrfToken: Ce.spelling("withXSRFToken")
    }, !0), r.method = (r.method || this.defaults.method || "get").toLowerCase();
    let i = a && x.merge(
      a.common,
      a[r.method]
    );
    a && x.forEach(
      ["delete", "get", "head", "post", "put", "patch", "common"],
      (p) => {
        delete a[p];
      }
    ), r.headers = ye.concat(i, a);
    const l = [];
    let u = !0;
    this.interceptors.request.forEach(function(b) {
      typeof b.runWhen == "function" && b.runWhen(r) === !1 || (u = u && b.synchronous, l.unshift(b.fulfilled, b.rejected));
    });
    const d = [];
    this.interceptors.response.forEach(function(b) {
      d.push(b.fulfilled, b.rejected);
    });
    let c, f = 0, y;
    if (!u) {
      const p = [Hs.bind(this), void 0];
      for (p.unshift(...l), p.push(...d), y = p.length, c = Promise.resolve(r); f < y; )
        c = c.then(p[f++], p[f++]);
      return c;
    }
    y = l.length;
    let m = r;
    for (; f < y; ) {
      const p = l[f++], b = l[f++];
      try {
        m = p(m);
      } catch (g) {
        b.call(this, g);
        break;
      }
    }
    try {
      c = Hs.call(this, m);
    } catch (p) {
      return Promise.reject(p);
    }
    for (f = 0, y = d.length; f < y; )
      c = c.then(d[f++], d[f++]);
    return c;
  }
  getUri(t) {
    t = ot(this.defaults, t);
    const r = ma(t.baseURL, t.url, t.allowAbsoluteUrls);
    return da(r, t.params, t.paramsSerializer);
  }
};
x.forEach(["delete", "get", "head", "options"], function(t) {
  nt.prototype[t] = function(r, n) {
    return this.request(ot(n || {}, {
      method: t,
      url: r,
      data: (n || {}).data
    }));
  };
});
x.forEach(["post", "put", "patch"], function(t) {
  function r(n) {
    return function(a, i, l) {
      return this.request(ot(l || {}, {
        method: t,
        headers: n ? {
          "Content-Type": "multipart/form-data"
        } : {},
        url: a,
        data: i
      }));
    };
  }
  nt.prototype[t] = r(), nt.prototype[t + "Form"] = r(!0);
});
let Fd = class va {
  constructor(t) {
    if (typeof t != "function")
      throw new TypeError("executor must be a function.");
    let r;
    this.promise = new Promise(function(a) {
      r = a;
    });
    const n = this;
    this.promise.then((s) => {
      if (!n._listeners) return;
      let a = n._listeners.length;
      for (; a-- > 0; )
        n._listeners[a](s);
      n._listeners = null;
    }), this.promise.then = (s) => {
      let a;
      const i = new Promise((l) => {
        n.subscribe(l), a = l;
      }).then(s);
      return i.cancel = function() {
        n.unsubscribe(a);
      }, i;
    }, t(function(a, i, l) {
      n.reason || (n.reason = new wt(a, i, l), r(n.reason));
    });
  }
  /**
   * Throws a `CanceledError` if cancellation has been requested.
   */
  throwIfRequested() {
    if (this.reason)
      throw this.reason;
  }
  /**
   * Subscribe to the cancel signal
   */
  subscribe(t) {
    if (this.reason) {
      t(this.reason);
      return;
    }
    this._listeners ? this._listeners.push(t) : this._listeners = [t];
  }
  /**
   * Unsubscribe from the cancel signal
   */
  unsubscribe(t) {
    if (!this._listeners)
      return;
    const r = this._listeners.indexOf(t);
    r !== -1 && this._listeners.splice(r, 1);
  }
  toAbortSignal() {
    const t = new AbortController(), r = (n) => {
      t.abort(n);
    };
    return this.subscribe(r), t.signal.unsubscribe = () => this.unsubscribe(r), t.signal;
  }
  /**
   * Returns an object that contains a new `CancelToken` and a function that, when called,
   * cancels the `CancelToken`.
   */
  static source() {
    let t;
    return {
      token: new va(function(s) {
        t = s;
      }),
      cancel: t
    };
  }
};
function Ud(e) {
  return function(r) {
    return e.apply(null, r);
  };
}
function $d(e) {
  return x.isObject(e) && e.isAxiosError === !0;
}
const Nn = {
  Continue: 100,
  SwitchingProtocols: 101,
  Processing: 102,
  EarlyHints: 103,
  Ok: 200,
  Created: 201,
  Accepted: 202,
  NonAuthoritativeInformation: 203,
  NoContent: 204,
  ResetContent: 205,
  PartialContent: 206,
  MultiStatus: 207,
  AlreadyReported: 208,
  ImUsed: 226,
  MultipleChoices: 300,
  MovedPermanently: 301,
  Found: 302,
  SeeOther: 303,
  NotModified: 304,
  UseProxy: 305,
  Unused: 306,
  TemporaryRedirect: 307,
  PermanentRedirect: 308,
  BadRequest: 400,
  Unauthorized: 401,
  PaymentRequired: 402,
  Forbidden: 403,
  NotFound: 404,
  MethodNotAllowed: 405,
  NotAcceptable: 406,
  ProxyAuthenticationRequired: 407,
  RequestTimeout: 408,
  Conflict: 409,
  Gone: 410,
  LengthRequired: 411,
  PreconditionFailed: 412,
  PayloadTooLarge: 413,
  UriTooLong: 414,
  UnsupportedMediaType: 415,
  RangeNotSatisfiable: 416,
  ExpectationFailed: 417,
  ImATeapot: 418,
  MisdirectedRequest: 421,
  UnprocessableEntity: 422,
  Locked: 423,
  FailedDependency: 424,
  TooEarly: 425,
  UpgradeRequired: 426,
  PreconditionRequired: 428,
  TooManyRequests: 429,
  RequestHeaderFieldsTooLarge: 431,
  UnavailableForLegalReasons: 451,
  InternalServerError: 500,
  NotImplemented: 501,
  BadGateway: 502,
  ServiceUnavailable: 503,
  GatewayTimeout: 504,
  HttpVersionNotSupported: 505,
  VariantAlsoNegotiates: 506,
  InsufficientStorage: 507,
  LoopDetected: 508,
  NotExtended: 510,
  NetworkAuthenticationRequired: 511,
  WebServerIsDown: 521,
  ConnectionTimedOut: 522,
  OriginIsUnreachable: 523,
  TimeoutOccurred: 524,
  SslHandshakeFailed: 525,
  InvalidSslCertificate: 526
};
Object.entries(Nn).forEach(([e, t]) => {
  Nn[t] = e;
});
function wa(e) {
  const t = new nt(e), r = Xo(nt.prototype.request, t);
  return x.extend(r, nt.prototype, t, { allOwnKeys: !0 }), x.extend(r, t, null, { allOwnKeys: !0 }), r.create = function(s) {
    return wa(ot(e, s));
  }, r;
}
const te = wa(qt);
te.Axios = nt;
te.CanceledError = wt;
te.CancelToken = Fd;
te.isCancel = ha;
te.VERSION = ba;
te.toFormData = Lr;
te.AxiosError = F;
te.Cancel = te.CanceledError;
te.all = function(t) {
  return Promise.all(t);
};
te.spread = Ud;
te.isAxiosError = $d;
te.mergeConfig = ot;
te.AxiosHeaders = ye;
te.formToJSON = (e) => fa(x.isHTMLForm(e) ? new FormData(e) : e);
te.getAdapter = xa.getAdapter;
te.HttpStatusCode = Nn;
te.default = te;
const {
  Axios: Uf,
  AxiosError: $f,
  CanceledError: Bf,
  isCancel: Vf,
  CancelToken: zf,
  VERSION: Wf,
  all: Hf,
  Cancel: qf,
  isAxiosError: Kf,
  spread: Jf,
  toFormData: Qf,
  AxiosHeaders: Gf,
  HttpStatusCode: Yf,
  formToJSON: Xf,
  getAdapter: Zf,
  mergeConfig: eh
} = te;
function Bd(e = {}) {
  const t = { ...zo, ...e };
  return te.create({
    baseURL: t.baseURL,
    timeout: t.timeout,
    withCredentials: t.withCredentials,
    headers: t.headers
  });
}
class Vd {
  constructor(t = {}) {
    this.isRefreshing = !1, this.refreshSubscribers = [], this.config = { ...zo, ...t }, this.instance = Bd(t), this.setupInterceptors();
  }
  /**
   * Get the underlying Axios instance
   */
  getAxiosInstance() {
    return this.instance;
  }
  /**
   * Setup request and response interceptors
   */
  setupInterceptors() {
    this.instance.interceptors.request.use(
      this.handleRequest.bind(this),
      this.handleRequestError.bind(this)
    ), this.instance.interceptors.response.use(
      this.handleResponse.bind(this),
      this.handleResponseError.bind(this)
    );
  }
  /**
   * Handle outgoing requests
   */
  handleRequest(t) {
    var n;
    const r = this.getAccessToken();
    return r && (t.headers.Authorization = `Bearer ${r}`), t.headers["X-Request-ID"] = this.generateRequestId(), t.headers["X-Request-Time"] = (/* @__PURE__ */ new Date()).toISOString(), process.env.NODE_ENV === "development" && console.log(`[API] ${(n = t.method) == null ? void 0 : n.toUpperCase()} ${t.url}`, {
      params: t.params,
      data: t.data
    }), t;
  }
  /**
   * Handle request errors
   */
  handleRequestError(t) {
    return console.error("[API] Request error:", t), Promise.reject(t);
  }
  /**
   * Handle incoming responses
   */
  handleResponse(t) {
    return process.env.NODE_ENV === "development" && console.log(`[API] Response ${t.status}:`, t.data), t;
  }
  /**
   * Handle response errors
   */
  async handleResponseError(t) {
    var a, i, l, u, d, c;
    const r = t.config;
    if (!t.response)
      return (i = (a = this.config).onNetworkError) == null || i.call(a, t), this.dispatchEvent(oe.NETWORK_ERROR, t), Promise.reject(this.createApiError(nn.NETWORK_ERROR, "Network error"));
    const { status: n, data: s } = t.response;
    if (n === Os.UNAUTHORIZED && !r._retry) {
      if (this.isRefreshing)
        return new Promise((f) => {
          this.refreshSubscribers.push((y) => {
            r.headers.Authorization = `Bearer ${y}`, f(this.instance(r));
          });
        });
      r._retry = !0, this.isRefreshing = !0;
      try {
        const f = await this.refreshToken();
        if (f)
          return this.notifyRefreshSubscribers(f), r.headers.Authorization = `Bearer ${f}`, this.instance(r);
      } catch (f) {
        return this.clearAuth(), (u = (l = this.config).onUnauthorized) == null || u.call(l), this.dispatchEvent(oe.UNAUTHORIZED), Promise.reject(f);
      } finally {
        this.isRefreshing = !1, this.refreshSubscribers = [];
      }
    }
    if (n === Os.FORBIDDEN && this.dispatchEvent(oe.PERMISSION_DENIED), n >= 500) {
      const f = this.extractApiError(s);
      (c = (d = this.config).onServerError) == null || c.call(d, f), this.dispatchEvent(oe.SERVER_ERROR, f);
    }
    return process.env.NODE_ENV === "development" && console.error(`[API] Error ${n}:`, s), Promise.reject(this.extractApiError(s, n));
  }
  /**
   * Refresh the access token
   */
  async refreshToken() {
    const t = localStorage.getItem(Q.REFRESH_TOKEN);
    if (!t) return null;
    try {
      const r = await te.post(
        `${this.config.baseURL}/api/auth/refresh`,
        { refreshToken: t },
        { withCredentials: !0 }
      ), { accessToken: n, refreshToken: s } = r.data;
      return localStorage.setItem(Q.ACCESS_TOKEN, n), s && localStorage.setItem(Q.REFRESH_TOKEN, s), this.dispatchEvent(oe.TOKEN_REFRESHED, { accessToken: n }), n;
    } catch {
      return null;
    }
  }
  /**
   * Notify all subscribers waiting for token refresh
   */
  notifyRefreshSubscribers(t) {
    this.refreshSubscribers.forEach((r) => r(t));
  }
  /**
   * Get access token from storage
   */
  getAccessToken() {
    return localStorage.getItem(Q.ACCESS_TOKEN);
  }
  /**
   * Clear authentication data
   */
  clearAuth() {
    localStorage.removeItem(Q.ACCESS_TOKEN), localStorage.removeItem(Q.REFRESH_TOKEN), localStorage.removeItem(Q.USER);
  }
  /**
   * Generate unique request ID
   */
  generateRequestId() {
    return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }
  /**
   * Create standardized API error
   */
  createApiError(t, r, n) {
    return { code: t, message: r, field: n };
  }
  /**
   * Extract API error from response data
   */
  extractApiError(t, r) {
    if (typeof t == "object" && t !== null) {
      const n = t;
      return {
        code: n.code || String(r) || nn.UNKNOWN_ERROR,
        message: n.message || "An error occurred",
        field: n.field,
        details: n.details
      };
    }
    return this.createApiError(nn.UNKNOWN_ERROR, String(t));
  }
  /**
   * Dispatch custom event
   */
  dispatchEvent(t, r) {
    window.dispatchEvent(new CustomEvent(t, { detail: r }));
  }
  // ==================== Public API Methods ====================
  /**
   * GET request
   */
  async get(t, r) {
    return (await this.instance.get(t, r)).data;
  }
  /**
   * POST request
   */
  async post(t, r, n) {
    return (await this.instance.post(t, r, n)).data;
  }
  /**
   * PUT request
   */
  async put(t, r, n) {
    return (await this.instance.put(t, r, n)).data;
  }
  /**
   * PATCH request
   */
  async patch(t, r, n) {
    return (await this.instance.patch(t, r, n)).data;
  }
  /**
   * DELETE request
   */
  async delete(t, r) {
    return (await this.instance.delete(t, r)).data;
  }
  /**
   * Upload file
   */
  async uploadFile(t, r, n = "file", s, a) {
    const i = new FormData();
    return i.append(n, r), s && Object.entries(s).forEach(([u, d]) => {
      i.append(u, String(d));
    }), (await this.instance.post(t, i, {
      headers: {
        "Content-Type": "multipart/form-data"
      },
      onUploadProgress: (u) => {
        if (u.total && a) {
          const d = Math.round(u.loaded * 100 / u.total);
          a(d);
        }
      }
    })).data;
  }
  /**
   * Download file
   */
  async downloadFile(t, r) {
    const n = await this.instance.get(t, {
      responseType: "blob"
    }), s = new Blob([n.data]), a = window.URL.createObjectURL(s), i = document.createElement("a");
    i.href = a, i.download = r || this.extractFilename(n) || "download", document.body.appendChild(i), i.click(), document.body.removeChild(i), window.URL.revokeObjectURL(a);
  }
  /**
   * Extract filename from response headers
   */
  extractFilename(t) {
    const r = t.headers["content-disposition"];
    if (r) {
      const n = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(r);
      if (n != null && n[1])
        return n[1].replace(/['"]/g, "");
    }
    return null;
  }
  /**
   * Set authorization token manually
   */
  setAuthToken(t) {
    localStorage.setItem(Q.ACCESS_TOKEN, t), this.instance.defaults.headers.common.Authorization = `Bearer ${t}`;
  }
  /**
   * Remove authorization token
   */
  removeAuthToken() {
    this.clearAuth(), delete this.instance.defaults.headers.common.Authorization;
  }
  /**
   * Update base URL
   */
  setBaseURL(t) {
    this.config.baseURL = t, this.instance.defaults.baseURL = t;
  }
}
const we = new Vd(), Ea = ho(null);
function zd({
  children: e,
  config: t,
  onAuthStateChange: r,
  onSessionExpired: n,
  onUnauthorized: s
}) {
  const a = uc(), i = xe(Vn), l = xe(Ho), u = xe(qo), d = xe(Ko), c = xe(Jo), f = xe(Qo), y = ke(() => ({ ...Gl, ...t }), [t]), m = _e(null), p = _e(null), b = _e(l), g = B(async (T) => {
    const M = await a(Dt({ credentials: T, apiClient: we }));
    if (Dt.fulfilled.match(M))
      return M.payload;
    throw new Error(M.payload || "Login failed");
  }, [a]), E = B(async () => {
    m.current && (clearTimeout(m.current), m.current = null), await a(ir(void 0));
  }, [a]), N = B(async () => {
    const T = await a(vr({ apiClient: we }));
    vr.rejected.match(T) && (n == null || n());
  }, [a, n]), _ = B((T) => i ? i.isAdmin ? !0 : (Array.isArray(T) ? T : [T]).some(
    (C) => i.permissions.some(($) => $.id === C || $.name === C)
  ) : !1, [i]), w = B((T) => i ? i.isAdmin ? !0 : (Array.isArray(T) ? T : [T]).some((C) => i.roles.includes(C)) : !1, [i]), S = B((T) => {
    a(Zl(T));
  }, [a]), A = B(() => {
    if (!f || !c) return;
    m.current && clearTimeout(m.current);
    const T = f - y.autoRefreshBuffer, M = Math.max(0, T - Date.now());
    M > 0 ? m.current = setTimeout(() => {
      N();
    }, M) : N();
  }, [f, c, y.autoRefreshBuffer, N]), D = B(() => {
    a(ec()), p.current && clearTimeout(p.current), l && y.sessionTimeout > 0 && (p.current = setTimeout(() => {
      a(tc()), n == null || n();
    }, y.sessionTimeout));
  }, [a, l, y.sessionTimeout, n]);
  me(() => (l && c && A(), () => {
    m.current && clearTimeout(m.current);
  }), [l, c, A]), me(() => {
    if (l) {
      const T = ["mousedown", "keydown", "scroll", "touchstart"];
      return T.forEach((M) => {
        window.addEventListener(M, D, { passive: !0 });
      }), D(), () => {
        T.forEach((M) => {
          window.removeEventListener(M, D);
        }), p.current && clearTimeout(p.current);
      };
    }
  }, [l, D]), me(() => {
    b.current !== l && (b.current = l, r == null || r(l, i));
  }, [l, i, r]), me(() => {
    const T = () => {
      s == null || s();
    }, M = () => {
      n == null || n();
    };
    return window.addEventListener(oe.UNAUTHORIZED, T), window.addEventListener(oe.SESSION_EXPIRED, M), () => {
      window.removeEventListener(oe.UNAUTHORIZED, T), window.removeEventListener(oe.SESSION_EXPIRED, M);
    };
  }, [s, n]);
  const U = ke(() => ({
    user: i,
    isAuthenticated: l,
    isLoading: u,
    error: d,
    login: g,
    logout: E,
    refreshToken: N,
    hasPermission: _,
    hasRole: w,
    updateUser: S
  }), [
    i,
    l,
    u,
    d,
    g,
    E,
    N,
    _,
    w,
    S
  ]);
  return /* @__PURE__ */ o.jsx(Ea.Provider, { value: U, children: e });
}
function Wd(e) {
  return /* @__PURE__ */ o.jsx(fi, { store: zn, children: /* @__PURE__ */ o.jsx(zd, { ...e }) });
}
function ka() {
  const e = po(Ea);
  if (!e)
    throw new Error("useAuthContext must be used within an AuthProvider");
  return e;
}
function Ks({
  logo: e,
  title: t = "پنل مدیریت",
  subtitle: r = "وارد حساب کاربری خود شوید",
  backgroundImage: n,
  footer: s,
  redirectPath: a = "/",
  showRememberMe: i = !0,
  showForgotPassword: l = !0,
  forgotPasswordUrl: u = "/forgot-password",
  className: d,
  onLoginSuccess: c,
  onLoginError: f
}) {
  var V, L;
  const y = go(), m = Fn(), { login: p, isLoading: b, error: g } = ka(), [E, N] = G(""), [_, w] = G(""), [S, A] = G(!1), [D, U] = G(!1), [T, M] = G(null), C = ((L = (V = m.state) == null ? void 0 : V.from) == null ? void 0 : L.pathname) || a, $ = B(async (Y) => {
    if (Y.preventDefault(), M(null), !E.trim()) {
      M("نام کاربری را وارد کنید");
      return;
    }
    if (!_) {
      M("رمز عبور را وارد کنید");
      return;
    }
    try {
      await p({
        username: E.trim(),
        password: _,
        rememberMe: S
      }), c == null || c(), y(C, { replace: !0 });
    } catch (Te) {
      const ue = Te instanceof Error ? Te.message : "خطا در ورود";
      M(ue), f == null || f(ue);
    }
  }, [E, _, S, p, y, C, c, f]), R = T || g;
  return /* @__PURE__ */ o.jsxs(
    "div",
    {
      className: j(
        "min-h-screen flex items-center justify-center",
        "bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900",
        "px-4 py-8",
        d
      ),
      style: n ? { backgroundImage: `url(${n})`, backgroundSize: "cover" } : void 0,
      children: [
        /* @__PURE__ */ o.jsxs("div", { className: "absolute inset-0 overflow-hidden pointer-events-none", children: [
          /* @__PURE__ */ o.jsx("div", { className: "absolute -top-40 -right-40 w-80 h-80 bg-purple-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob" }),
          /* @__PURE__ */ o.jsx("div", { className: "absolute -bottom-40 -left-40 w-80 h-80 bg-cyan-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-2000" }),
          /* @__PURE__ */ o.jsx("div", { className: "absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-pink-500 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob animation-delay-4000" })
        ] }),
        /* @__PURE__ */ o.jsxs("div", { className: "relative w-full max-w-md", children: [
          /* @__PURE__ */ o.jsxs("div", { className: "backdrop-blur-xl bg-white/10 rounded-3xl shadow-2xl border border-white/20 p-8", children: [
            /* @__PURE__ */ o.jsxs("div", { className: "text-center mb-8", children: [
              e && /* @__PURE__ */ o.jsx("div", { className: "mb-4 flex justify-center", children: typeof e == "string" ? /* @__PURE__ */ o.jsx("img", { src: e, alt: "Logo", className: "h-16 w-auto" }) : e }),
              /* @__PURE__ */ o.jsx("h1", { className: "text-3xl font-bold text-white mb-2", children: t }),
              /* @__PURE__ */ o.jsx("p", { className: "text-gray-300", children: r })
            ] }),
            R && /* @__PURE__ */ o.jsxs("div", { className: "mb-6 p-4 bg-red-500/20 border border-red-500/30 rounded-xl text-red-200 text-sm text-center backdrop-blur-sm", children: [
              /* @__PURE__ */ o.jsx("svg", { className: "inline-block w-5 h-5 mr-2", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" }) }),
              R
            ] }),
            /* @__PURE__ */ o.jsxs("form", { onSubmit: $, className: "space-y-5", children: [
              /* @__PURE__ */ o.jsxs("div", { children: [
                /* @__PURE__ */ o.jsx("label", { htmlFor: "username", className: "block text-sm font-medium text-gray-200 mb-2", children: "نام کاربری" }),
                /* @__PURE__ */ o.jsxs("div", { className: "relative", children: [
                  /* @__PURE__ */ o.jsx(
                    "input",
                    {
                      id: "username",
                      type: "text",
                      value: E,
                      onChange: (Y) => N(Y.target.value),
                      className: j(
                        "w-full px-4 py-3 pr-11",
                        "bg-white/10 backdrop-blur-sm",
                        "border border-white/20 rounded-xl",
                        "text-white placeholder-gray-400",
                        "focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent",
                        "transition-all duration-200"
                      ),
                      placeholder: "نام کاربری یا ایمیل",
                      autoComplete: "username",
                      disabled: b
                    }
                  ),
                  /* @__PURE__ */ o.jsx(
                    "svg",
                    {
                      className: "absolute right-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400",
                      fill: "none",
                      stroke: "currentColor",
                      viewBox: "0 0 24 24",
                      children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" })
                    }
                  )
                ] })
              ] }),
              /* @__PURE__ */ o.jsxs("div", { children: [
                /* @__PURE__ */ o.jsx("label", { htmlFor: "password", className: "block text-sm font-medium text-gray-200 mb-2", children: "رمز عبور" }),
                /* @__PURE__ */ o.jsxs("div", { className: "relative", children: [
                  /* @__PURE__ */ o.jsx(
                    "input",
                    {
                      id: "password",
                      type: D ? "text" : "password",
                      value: _,
                      onChange: (Y) => w(Y.target.value),
                      className: j(
                        "w-full px-4 py-3 pr-11 pl-11",
                        "bg-white/10 backdrop-blur-sm",
                        "border border-white/20 rounded-xl",
                        "text-white placeholder-gray-400",
                        "focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent",
                        "transition-all duration-200"
                      ),
                      placeholder: "رمز عبور",
                      autoComplete: "current-password",
                      disabled: b
                    }
                  ),
                  /* @__PURE__ */ o.jsx(
                    "svg",
                    {
                      className: "absolute right-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400",
                      fill: "none",
                      stroke: "currentColor",
                      viewBox: "0 0 24 24",
                      children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" })
                    }
                  ),
                  /* @__PURE__ */ o.jsx(
                    "button",
                    {
                      type: "button",
                      onClick: () => U(!D),
                      className: "absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-white transition-colors",
                      tabIndex: -1,
                      children: D ? /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" }) }) : /* @__PURE__ */ o.jsxs("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: [
                        /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M15 12a3 3 0 11-6 0 3 3 0 016 0z" }),
                        /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" })
                      ] })
                    }
                  )
                ] })
              ] }),
              /* @__PURE__ */ o.jsxs("div", { className: "flex items-center justify-between", children: [
                i && /* @__PURE__ */ o.jsxs("label", { className: "flex items-center gap-2 cursor-pointer group", children: [
                  /* @__PURE__ */ o.jsx(
                    "input",
                    {
                      type: "checkbox",
                      checked: S,
                      onChange: (Y) => A(Y.target.checked),
                      className: "w-4 h-4 rounded border-white/30 bg-white/10 text-purple-500 focus:ring-purple-500 focus:ring-offset-0",
                      disabled: b
                    }
                  ),
                  /* @__PURE__ */ o.jsx("span", { className: "text-sm text-gray-300 group-hover:text-white transition-colors", children: "مرا به خاطر بسپار" })
                ] }),
                l && /* @__PURE__ */ o.jsx(
                  "a",
                  {
                    href: u,
                    className: "text-sm text-purple-300 hover:text-purple-200 transition-colors",
                    children: "فراموشی رمز عبور؟"
                  }
                )
              ] }),
              /* @__PURE__ */ o.jsx(
                "button",
                {
                  type: "submit",
                  disabled: b,
                  className: j(
                    "w-full py-3 px-4 rounded-xl font-medium text-white",
                    "bg-gradient-to-r from-purple-600 to-pink-600",
                    "hover:from-purple-500 hover:to-pink-500",
                    "focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2 focus:ring-offset-transparent",
                    "transition-all duration-200",
                    "shadow-lg shadow-purple-500/30",
                    b && "opacity-75 cursor-not-allowed"
                  ),
                  children: b ? /* @__PURE__ */ o.jsxs("span", { className: "flex items-center justify-center gap-2", children: [
                    /* @__PURE__ */ o.jsxs("svg", { className: "animate-spin h-5 w-5", fill: "none", viewBox: "0 0 24 24", children: [
                      /* @__PURE__ */ o.jsx("circle", { className: "opacity-25", cx: "12", cy: "12", r: "10", stroke: "currentColor", strokeWidth: "4" }),
                      /* @__PURE__ */ o.jsx("path", { className: "opacity-75", fill: "currentColor", d: "M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" })
                    ] }),
                    "در حال ورود..."
                  ] }) : "ورود به سیستم"
                }
              )
            ] }),
            s && /* @__PURE__ */ o.jsx("div", { className: "mt-8 text-center text-sm text-gray-400", children: s })
          ] }),
          /* @__PURE__ */ o.jsxs("p", { className: "mt-6 text-center text-xs text-gray-500", children: [
            "© ",
            (/* @__PURE__ */ new Date()).getFullYear(),
            " Neo BPMS. تمام حقوق محفوظ است."
          ] })
        ] }),
        /* @__PURE__ */ o.jsx("style", { children: `
        @keyframes blob {
          0%, 100% { transform: translate(0, 0) scale(1); }
          25% { transform: translate(20px, -30px) scale(1.1); }
          50% { transform: translate(-20px, 20px) scale(0.9); }
          75% { transform: translate(30px, 30px) scale(1.05); }
        }
        .animate-blob {
          animation: blob 7s infinite;
        }
        .animation-delay-2000 {
          animation-delay: 2s;
        }
        .animation-delay-4000 {
          animation-delay: 4s;
        }
      ` })
      ]
    }
  );
}
const Hd = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  LoginPage: Ks,
  default: Ks
}, Symbol.toStringTag, { value: "Module" }));
function qd({ title: e, value: t, change: r, changeLabel: n, icon: s, color: a = "blue", onClick: i }) {
  const l = {
    blue: "from-blue-500 to-blue-600",
    green: "from-emerald-500 to-emerald-600",
    purple: "from-purple-500 to-purple-600",
    orange: "from-orange-500 to-orange-600",
    red: "from-red-500 to-red-600",
    cyan: "from-cyan-500 to-cyan-600"
  }, u = r && r > 0, d = r && r < 0;
  return /* @__PURE__ */ o.jsxs(
    "div",
    {
      onClick: i,
      className: j(
        "relative overflow-hidden rounded-2xl p-6",
        "bg-gradient-to-br",
        l[a],
        "text-white shadow-lg",
        "transition-all duration-300",
        i && "cursor-pointer hover:scale-[1.02] hover:shadow-xl"
      ),
      children: [
        /* @__PURE__ */ o.jsx("div", { className: "absolute top-0 left-0 w-32 h-32 bg-white/10 rounded-full -translate-x-1/2 -translate-y-1/2" }),
        /* @__PURE__ */ o.jsx("div", { className: "absolute bottom-0 right-0 w-24 h-24 bg-white/10 rounded-full translate-x-1/2 translate-y-1/2" }),
        /* @__PURE__ */ o.jsxs("div", { className: "relative", children: [
          /* @__PURE__ */ o.jsxs("div", { className: "flex items-center justify-between mb-4", children: [
            /* @__PURE__ */ o.jsx("span", { className: "text-sm font-medium text-white/80", children: e }),
            s && /* @__PURE__ */ o.jsx("div", { className: "text-white/80", children: s })
          ] }),
          /* @__PURE__ */ o.jsx("div", { className: "text-3xl font-bold mb-2", style: { direction: "ltr", textAlign: "right" }, children: typeof t == "number" ? t.toLocaleString("fa-IR") : t }),
          r !== void 0 && /* @__PURE__ */ o.jsxs("div", { className: "flex items-center gap-1 text-sm", children: [
            u && /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M5 10l7-7m0 0l7 7m-7-7v18" }) }),
            d && /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M19 14l-7 7m0 0l-7-7m7 7V3" }) }),
            /* @__PURE__ */ o.jsxs("span", { className: j(
              "font-medium",
              u && "text-green-200",
              d && "text-red-200",
              !r && "text-white/60"
            ), children: [
              Math.abs(r),
              "%"
            ] }),
            n && /* @__PURE__ */ o.jsx("span", { className: "text-white/60", children: n })
          ] })
        ] })
      ]
    }
  );
}
function Kd({ label: e, icon: t, onClick: r, color: n }) {
  return /* @__PURE__ */ o.jsxs(
    "button",
    {
      onClick: r,
      className: j(
        "flex flex-col items-center gap-2 p-4 rounded-xl",
        "bg-white dark:bg-gray-800",
        "border border-gray-200 dark:border-gray-700",
        "hover:border-blue-300 dark:hover:border-blue-600",
        "hover:shadow-md",
        "transition-all duration-200",
        "group"
      ),
      children: [
        /* @__PURE__ */ o.jsx(
          "div",
          {
            className: j(
              "w-12 h-12 rounded-xl flex items-center justify-center",
              "bg-gradient-to-br from-blue-50 to-blue-100 dark:from-blue-900/30 dark:to-blue-800/30",
              "group-hover:scale-110 transition-transform duration-200"
            ),
            style: n ? { background: `linear-gradient(135deg, ${n}20, ${n}30)` } : void 0,
            children: t
          }
        ),
        /* @__PURE__ */ o.jsx("span", { className: "text-sm font-medium text-gray-700 dark:text-gray-300", children: e })
      ]
    }
  );
}
function Jd({ title: e, description: t, time: r, icon: n, type: s = "info" }) {
  const a = {
    success: "bg-green-100 text-green-600 dark:bg-green-900/30 dark:text-green-400",
    warning: "bg-yellow-100 text-yellow-600 dark:bg-yellow-900/30 dark:text-yellow-400",
    error: "bg-red-100 text-red-600 dark:bg-red-900/30 dark:text-red-400",
    info: "bg-blue-100 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400"
  };
  return /* @__PURE__ */ o.jsxs("div", { className: "flex items-start gap-3 p-3 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800/50 transition-colors", children: [
    /* @__PURE__ */ o.jsx("div", { className: j("w-10 h-10 rounded-full flex items-center justify-center flex-shrink-0", a[s]), children: n || /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" }) }) }),
    /* @__PURE__ */ o.jsxs("div", { className: "flex-1 min-w-0", children: [
      /* @__PURE__ */ o.jsx("p", { className: "text-sm font-medium text-gray-900 dark:text-white", children: e }),
      t && /* @__PURE__ */ o.jsx("p", { className: "text-xs text-gray-500 dark:text-gray-400 mt-0.5", children: t }),
      /* @__PURE__ */ o.jsx("p", { className: "text-xs text-gray-400 dark:text-gray-500 mt-1", children: r })
    ] })
  ] });
}
function Qd() {
  return /* @__PURE__ */ o.jsxs("div", { className: "animate-pulse space-y-6", children: [
    /* @__PURE__ */ o.jsx("div", { className: "h-8 w-64 bg-gray-200 dark:bg-gray-700 rounded" }),
    /* @__PURE__ */ o.jsx("div", { className: "grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6", children: [1, 2, 3, 4].map((e) => /* @__PURE__ */ o.jsx("div", { className: "h-36 bg-gray-200 dark:bg-gray-700 rounded-2xl" }, e)) }),
    /* @__PURE__ */ o.jsxs("div", { className: "grid grid-cols-1 lg:grid-cols-3 gap-6", children: [
      /* @__PURE__ */ o.jsx("div", { className: "lg:col-span-2 h-64 bg-gray-200 dark:bg-gray-700 rounded-2xl" }),
      /* @__PURE__ */ o.jsx("div", { className: "h-64 bg-gray-200 dark:bg-gray-700 rounded-2xl" })
    ] })
  ] });
}
function Js({
  title: e = "داشبورد",
  welcomeMessage: t,
  userName: r,
  stats: n = [],
  quickActions: s = [],
  recentActivity: a = [],
  widgets: i,
  isLoading: l = !1,
  className: u
}) {
  const d = ke(() => {
    const c = (/* @__PURE__ */ new Date()).getHours();
    return c < 12 ? "صبح بخیر" : c < 17 ? "عصر بخیر" : "شب بخیر";
  }, []);
  return l ? /* @__PURE__ */ o.jsx("div", { className: j("p-6", u), children: /* @__PURE__ */ o.jsx(Qd, {}) }) : /* @__PURE__ */ o.jsxs("div", { className: j("p-6 space-y-6", u), children: [
    /* @__PURE__ */ o.jsxs("div", { className: "flex flex-col md:flex-row md:items-center md:justify-between gap-4", children: [
      /* @__PURE__ */ o.jsxs("div", { children: [
        /* @__PURE__ */ o.jsx("h1", { className: "text-2xl font-bold text-gray-900 dark:text-white", children: e }),
        (t || r) && /* @__PURE__ */ o.jsx("p", { className: "text-gray-500 dark:text-gray-400 mt-1", children: t || `${d}${r ? `، ${r}` : ""}` })
      ] }),
      /* @__PURE__ */ o.jsx("div", { className: "text-sm text-gray-500 dark:text-gray-400", children: (/* @__PURE__ */ new Date()).toLocaleDateString("fa-IR", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric"
      }) })
    ] }),
    n.length > 0 && /* @__PURE__ */ o.jsx("div", { className: "grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6", children: n.map((c, f) => /* @__PURE__ */ o.jsx(qd, { ...c }, f)) }),
    s.length > 0 && /* @__PURE__ */ o.jsxs("div", { className: "bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700", children: [
      /* @__PURE__ */ o.jsx("h2", { className: "text-lg font-semibold text-gray-900 dark:text-white mb-4", children: "دسترسی سریع" }),
      /* @__PURE__ */ o.jsx("div", { className: "grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4", children: s.map((c, f) => /* @__PURE__ */ o.jsx(Kd, { ...c }, f)) })
    ] }),
    /* @__PURE__ */ o.jsxs("div", { className: "grid grid-cols-1 lg:grid-cols-3 gap-6", children: [
      /* @__PURE__ */ o.jsx("div", { className: "lg:col-span-2", children: i || /* @__PURE__ */ o.jsx("div", { className: "bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700 h-full min-h-[300px] flex items-center justify-center", children: /* @__PURE__ */ o.jsxs("div", { className: "text-center text-gray-400", children: [
        /* @__PURE__ */ o.jsx("svg", { className: "w-16 h-16 mx-auto mb-4 opacity-50", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 1.5, d: "M4 5a1 1 0 011-1h14a1 1 0 011 1v2a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM4 13a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H5a1 1 0 01-1-1v-6zM16 13a1 1 0 011-1h2a1 1 0 011 1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-6z" }) }),
        /* @__PURE__ */ o.jsx("p", { className: "text-sm", children: "ویجت‌های سفارشی خود را اینجا اضافه کنید" })
      ] }) }) }),
      /* @__PURE__ */ o.jsxs("div", { className: "bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700", children: [
        /* @__PURE__ */ o.jsx("h2", { className: "text-lg font-semibold text-gray-900 dark:text-white mb-4", children: "فعالیت‌های اخیر" }),
        a.length > 0 ? /* @__PURE__ */ o.jsx("div", { className: "space-y-1 max-h-80 overflow-y-auto", children: a.map((c) => /* @__PURE__ */ o.jsx(Jd, { ...c }, c.id)) }) : /* @__PURE__ */ o.jsxs("div", { className: "text-center py-8 text-gray-400", children: [
          /* @__PURE__ */ o.jsx("svg", { className: "w-12 h-12 mx-auto mb-3 opacity-50", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 1.5, d: "M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" }) }),
          /* @__PURE__ */ o.jsx("p", { className: "text-sm", children: "فعالیتی ثبت نشده است" })
        ] })
      ] })
    ] })
  ] });
}
const Gd = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  DashboardPage: Js,
  default: Js
}, Symbol.toStringTag, { value: "Module" }));
function Qs({
  title: e = "صفحه پیدا نشد",
  description: t = "صفحه‌ای که به دنبال آن هستید وجود ندارد یا منتقل شده است.",
  showBackButton: r = !0,
  showHomeButton: n = !0,
  homePath: s = "/",
  illustration: a,
  className: i
}) {
  const l = go();
  return /* @__PURE__ */ o.jsx(
    "div",
    {
      className: j(
        "min-h-screen flex items-center justify-center p-4",
        "bg-gradient-to-br from-gray-50 to-gray-100 dark:from-gray-900 dark:to-gray-800",
        i
      ),
      children: /* @__PURE__ */ o.jsxs("div", { className: "text-center max-w-md", children: [
        a || /* @__PURE__ */ o.jsxs("div", { className: "relative mb-8", children: [
          /* @__PURE__ */ o.jsx("div", { className: "text-[150px] font-black text-gray-200 dark:text-gray-700 leading-none select-none", children: "404" }),
          /* @__PURE__ */ o.jsx("div", { className: "absolute inset-0 flex items-center justify-center", children: /* @__PURE__ */ o.jsx(
            "svg",
            {
              className: "w-32 h-32 text-purple-500 opacity-80",
              fill: "none",
              stroke: "currentColor",
              viewBox: "0 0 24 24",
              children: /* @__PURE__ */ o.jsx(
                "path",
                {
                  strokeLinecap: "round",
                  strokeLinejoin: "round",
                  strokeWidth: 1.5,
                  d: "M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                }
              )
            }
          ) })
        ] }),
        /* @__PURE__ */ o.jsx("h1", { className: "text-3xl font-bold text-gray-900 dark:text-white mb-3", children: e }),
        /* @__PURE__ */ o.jsx("p", { className: "text-gray-500 dark:text-gray-400 mb-8", children: t }),
        /* @__PURE__ */ o.jsxs("div", { className: "flex flex-col sm:flex-row items-center justify-center gap-3", children: [
          r && /* @__PURE__ */ o.jsxs(
            "button",
            {
              onClick: () => l(-1),
              className: j(
                "px-6 py-2.5 rounded-xl font-medium",
                "border border-gray-300 dark:border-gray-600",
                "text-gray-700 dark:text-gray-300",
                "hover:bg-gray-50 dark:hover:bg-gray-800",
                "transition-colors duration-200",
                "flex items-center gap-2"
              ),
              children: [
                /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5 rotate-180", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M14 5l7 7m0 0l-7 7m7-7H3" }) }),
                "بازگشت"
              ]
            }
          ),
          n && /* @__PURE__ */ o.jsxs(
            "button",
            {
              onClick: () => l(s),
              className: j(
                "px-6 py-2.5 rounded-xl font-medium",
                "bg-gradient-to-r from-purple-600 to-pink-600",
                "text-white",
                "hover:from-purple-500 hover:to-pink-500",
                "shadow-lg shadow-purple-500/30",
                "transition-all duration-200",
                "flex items-center gap-2"
              ),
              children: [
                /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" }) }),
                "صفحه اصلی"
              ]
            }
          )
        ] })
      ] })
    }
  );
}
const Yd = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  NotFoundPage: Qs,
  default: Qs
}, Symbol.toStringTag, { value: "Module" })), Na = [
  { value: "1m", label: "۱ دقیقه", labelEn: "1 min", seconds: 60 },
  { value: "5m", label: "۵ دقیقه", labelEn: "5 min", seconds: 300 },
  { value: "15m", label: "۱۵ دقیقه", labelEn: "15 min", seconds: 900 },
  { value: "1h", label: "۱ ساعت", labelEn: "1 hour", seconds: 3600 },
  { value: "24h", label: "۲۴ ساعت", labelEn: "24 hours", seconds: 86400 }
], Xd = ({
  value: e,
  onChange: t,
  size: r = "md",
  showLabels: n = !0,
  className: s
}) => {
  const a = {
    sm: "px-2 py-1 text-xs",
    md: "px-3 py-1.5 text-sm",
    lg: "px-4 py-2 text-base"
  };
  return /* @__PURE__ */ o.jsx(
    "div",
    {
      className: j(
        "inline-flex rounded-xl p-1",
        "bg-gray-100 dark:bg-gray-800",
        s
      ),
      children: Na.map((i) => {
        const l = e === i.value;
        return /* @__PURE__ */ o.jsx(
          "button",
          {
            onClick: () => t(i.value),
            className: j(
              "rounded-lg font-medium transition-all duration-200",
              a[r],
              l ? "bg-white dark:bg-gray-700 text-purple-600 dark:text-purple-400 shadow-sm" : "text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white"
            ),
            children: n ? i.label : i.labelEn
          },
          i.value
        );
      })
    }
  );
}, Gs = {
  primary: { stroke: "#a855f7", fill: "rgba(168, 85, 247, 0.2)" },
  success: { stroke: "#10b981", fill: "rgba(16, 185, 129, 0.2)" },
  warning: { stroke: "#f59e0b", fill: "rgba(245, 158, 11, 0.2)" },
  danger: { stroke: "#ef4444", fill: "rgba(239, 68, 68, 0.2)" },
  info: { stroke: "#3b82f6", fill: "rgba(59, 130, 246, 0.2)" }
}, ja = ({
  data: e,
  width: t = 100,
  height: r = 32,
  color: n = "primary",
  showArea: s = !0,
  strokeWidth: a = 2,
  className: i
}) => {
  const { pathD: l, areaD: u, colors: d } = ke(() => {
    if (!e || e.length < 2)
      return { pathD: "", areaD: "", colors: Gs.primary };
    const c = Gs[n] || { stroke: n, fill: `${n}33` }, f = a, y = t - f * 2, m = r - f * 2, p = Math.min(...e), g = Math.max(...e) - p || 1, E = e.map((w, S) => {
      const A = f + S / (e.length - 1) * y, D = f + m - (w - p) / g * m;
      return { x: A, y: D };
    }), N = E.map((w, S) => `${S === 0 ? "M" : "L"} ${w.x} ${w.y}`).join(" "), _ = `${N} L ${E[E.length - 1].x} ${r - f} L ${f} ${r - f} Z`;
    return { pathD: N, areaD: _, colors: c };
  }, [e, t, r, n, a]);
  return !e || e.length < 2 ? /* @__PURE__ */ o.jsx(
    "div",
    {
      className: j("flex items-center justify-center text-gray-400 text-xs", i),
      style: { width: t, height: r },
      children: "—"
    }
  ) : /* @__PURE__ */ o.jsxs(
    "svg",
    {
      width: t,
      height: r,
      className: j("overflow-visible", i),
      viewBox: `0 0 ${t} ${r}`,
      children: [
        s && /* @__PURE__ */ o.jsx("defs", { children: /* @__PURE__ */ o.jsxs("linearGradient", { id: `sparkline-gradient-${n}`, x1: "0", y1: "0", x2: "0", y2: "1", children: [
          /* @__PURE__ */ o.jsx("stop", { offset: "0%", stopColor: d.fill }),
          /* @__PURE__ */ o.jsx("stop", { offset: "100%", stopColor: "transparent" })
        ] }) }),
        s && /* @__PURE__ */ o.jsx(
          "path",
          {
            d: u,
            fill: `url(#sparkline-gradient-${n})`,
            opacity: 0.5
          }
        ),
        /* @__PURE__ */ o.jsx(
          "path",
          {
            d: l,
            fill: "none",
            stroke: d.stroke,
            strokeWidth: a,
            strokeLinecap: "round",
            strokeLinejoin: "round"
          }
        ),
        e.length > 0 && /* @__PURE__ */ o.jsx(
          "circle",
          {
            cx: t - a,
            cy: a + (r - a * 2) - (e[e.length - 1] - Math.min(...e)) / (Math.max(...e) - Math.min(...e) || 1) * (r - a * 2),
            r: a + 1,
            fill: d.stroke
          }
        )
      ]
    }
  );
}, Zd = (e, t) => t === "currency" ? new Intl.NumberFormat("fa-IR").format(e) + " ریال" : t === "percent" ? e.toFixed(1) + "%" : e >= 1e6 ? (e / 1e6).toFixed(1) + "M" : e >= 1e3 ? (e / 1e3).toFixed(1) + "K" : new Intl.NumberFormat("fa-IR").format(e), eu = ({
  direction: e,
  percent: t
}) => {
  const r = {
    up: "text-emerald-500",
    down: "text-red-500",
    stable: "text-gray-400"
  }, n = {
    up: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M7 17l9.2-9.2M17 17V7H7" }) }),
    down: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M17 7l-9.2 9.2M7 7v10h10" }) }),
    stable: /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M5 12h14" }) })
  };
  return /* @__PURE__ */ o.jsxs("div", { className: j("flex items-center gap-1", r[e]), children: [
    n[e],
    t !== void 0 && /* @__PURE__ */ o.jsxs("span", { className: "text-xs font-medium", children: [
      t > 0 ? "+" : "",
      t.toFixed(1),
      "%"
    ] })
  ] });
}, tu = {
  primary: {
    bg: "bg-purple-50 dark:bg-purple-900/20",
    text: "text-purple-600 dark:text-purple-400",
    icon: "bg-purple-100 dark:bg-purple-800"
  },
  success: {
    bg: "bg-emerald-50 dark:bg-emerald-900/20",
    text: "text-emerald-600 dark:text-emerald-400",
    icon: "bg-emerald-100 dark:bg-emerald-800"
  },
  warning: {
    bg: "bg-amber-50 dark:bg-amber-900/20",
    text: "text-amber-600 dark:text-amber-400",
    icon: "bg-amber-100 dark:bg-amber-800"
  },
  danger: {
    bg: "bg-red-50 dark:bg-red-900/20",
    text: "text-red-600 dark:text-red-400",
    icon: "bg-red-100 dark:bg-red-800"
  },
  info: {
    bg: "bg-blue-50 dark:bg-blue-900/20",
    text: "text-blue-600 dark:text-blue-400",
    icon: "bg-blue-100 dark:bg-blue-800"
  }
}, cn = {
  users: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" }) }),
  money: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" }) }),
  orders: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" }) }),
  activity: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M13 10V3L4 14h7v7l9-11h-7z" }) })
}, dn = ({
  metric: e,
  loading: t = !1,
  onClick: r,
  className: n
}) => {
  const s = tu[e.color || "primary"];
  return t ? /* @__PURE__ */ o.jsx(
    "div",
    {
      className: j(
        "bg-white dark:bg-gray-800 rounded-2xl p-5",
        "border border-gray-200 dark:border-gray-700",
        "animate-pulse",
        n
      ),
      children: /* @__PURE__ */ o.jsxs("div", { className: "flex items-start justify-between", children: [
        /* @__PURE__ */ o.jsxs("div", { className: "space-y-3 flex-1", children: [
          /* @__PURE__ */ o.jsx("div", { className: "h-4 bg-gray-200 dark:bg-gray-700 rounded w-24" }),
          /* @__PURE__ */ o.jsx("div", { className: "h-8 bg-gray-200 dark:bg-gray-700 rounded w-32" }),
          /* @__PURE__ */ o.jsx("div", { className: "h-3 bg-gray-200 dark:bg-gray-700 rounded w-16" })
        ] }),
        /* @__PURE__ */ o.jsx("div", { className: "w-24 h-10 bg-gray-200 dark:bg-gray-700 rounded" })
      ] })
    }
  ) : /* @__PURE__ */ o.jsx(
    "div",
    {
      onClick: r,
      className: j(
        "bg-white dark:bg-gray-800 rounded-2xl p-5",
        "border border-gray-200 dark:border-gray-700",
        "transition-all duration-200",
        r && "cursor-pointer hover:shadow-lg hover:border-purple-300 dark:hover:border-purple-600",
        n
      ),
      children: /* @__PURE__ */ o.jsxs("div", { className: "flex items-start justify-between gap-4", children: [
        /* @__PURE__ */ o.jsxs("div", { className: "flex-1 min-w-0", children: [
          /* @__PURE__ */ o.jsxs("div", { className: "flex items-center gap-3 mb-3", children: [
            /* @__PURE__ */ o.jsx(
              "div",
              {
                className: j(
                  "w-10 h-10 rounded-xl flex items-center justify-center",
                  s.icon,
                  s.text
                ),
                children: e.icon && cn[e.icon] || cn.activity
              }
            ),
            /* @__PURE__ */ o.jsxs("div", { children: [
              /* @__PURE__ */ o.jsx("h3", { className: "text-sm font-medium text-gray-500 dark:text-gray-400", children: e.name }),
              e.category && /* @__PURE__ */ o.jsx("span", { className: "text-xs text-gray-400", children: e.category })
            ] })
          ] }),
          /* @__PURE__ */ o.jsxs("div", { className: "flex items-end gap-2", children: [
            /* @__PURE__ */ o.jsx("span", { className: "text-2xl font-bold text-gray-900 dark:text-white", children: Zd(e.value, e.format) }),
            e.unit && /* @__PURE__ */ o.jsx("span", { className: "text-sm text-gray-500 dark:text-gray-400 mb-1", children: e.unit })
          ] }),
          e.trend && /* @__PURE__ */ o.jsx("div", { className: "mt-2", children: /* @__PURE__ */ o.jsx(
            eu,
            {
              direction: e.trend,
              percent: e.trendPercent
            }
          ) })
        ] }),
        e.sparklineData && e.sparklineData.length > 0 && /* @__PURE__ */ o.jsx("div", { className: "flex-shrink-0", children: /* @__PURE__ */ o.jsx(
          ja,
          {
            data: e.sparklineData,
            width: 100,
            height: 40,
            color: e.color || "primary"
          }
        ) })
      ] })
    }
  );
}, Ys = (e) => {
  const t = Na.find((a) => a.value === e), r = Math.min(20, Math.floor(((t == null ? void 0 : t.seconds) || 300) / 15)), n = (a, i) => Array.from(
    { length: r },
    () => a + (Math.random() - 0.5) * i
  );
  return [
    {
      id: "new-customers",
      name: "مشتریان جدید",
      nameEn: "New Customers",
      value: Math.floor(Math.random() * 150) + 50,
      previousValue: Math.floor(Math.random() * 150) + 50,
      trend: Math.random() > 0.5 ? "up" : "down",
      trendPercent: Math.random() * 20 - 10,
      sparklineData: n(100, 50),
      color: "primary",
      icon: "users",
      category: "مشتریان"
    },
    {
      id: "active-sessions",
      name: "نشست‌های فعال",
      nameEn: "Active Sessions",
      value: Math.floor(Math.random() * 500) + 200,
      previousValue: Math.floor(Math.random() * 500) + 200,
      trend: Math.random() > 0.3 ? "up" : "stable",
      trendPercent: Math.random() * 15,
      sparklineData: n(350, 100),
      color: "success",
      icon: "activity",
      category: "سیستم"
    },
    {
      id: "revenue",
      name: "درآمد",
      nameEn: "Revenue",
      value: Math.floor(Math.random() * 5e7) + 1e7,
      previousValue: Math.floor(Math.random() * 5e7) + 1e7,
      format: "currency",
      trend: Math.random() > 0.4 ? "up" : "down",
      trendPercent: Math.random() * 25 - 5,
      sparklineData: n(3e7, 1e7),
      color: "success",
      icon: "money",
      category: "مالی"
    },
    {
      id: "orders",
      name: "سفارشات",
      nameEn: "Orders",
      value: Math.floor(Math.random() * 200) + 30,
      previousValue: Math.floor(Math.random() * 200) + 30,
      trend: Math.random() > 0.5 ? "up" : "stable",
      trendPercent: Math.random() * 12,
      sparklineData: n(100, 40),
      color: "info",
      icon: "orders",
      category: "فروش"
    },
    {
      id: "conversion-rate",
      name: "نرخ تبدیل",
      nameEn: "Conversion Rate",
      value: Math.random() * 5 + 2,
      previousValue: Math.random() * 5 + 2,
      format: "percent",
      trend: Math.random() > 0.6 ? "up" : "down",
      trendPercent: Math.random() * 8 - 2,
      sparklineData: n(3.5, 1),
      color: "warning",
      icon: "activity",
      category: "بازاریابی"
    },
    {
      id: "support-tickets",
      name: "تیکت‌های پشتیبانی",
      nameEn: "Support Tickets",
      value: Math.floor(Math.random() * 30) + 5,
      previousValue: Math.floor(Math.random() * 30) + 5,
      trend: Math.random() > 0.7 ? "down" : "up",
      trendPercent: Math.random() * 10 - 5,
      sparklineData: n(15, 8),
      color: "danger",
      icon: "activity",
      category: "پشتیبانی"
    }
  ];
}, ru = ({
  apiUrl: e = "/api/live-operations",
  initialTimeFrame: t = "5m",
  refreshInterval: r = 5e3,
  fetchFn: n
} = {}) => {
  const [s, a] = G(null), [i, l] = G(!0), [u, d] = G(null), [c, f] = G(t), [y, m] = G(!1), [p, b] = G(null), g = _e(null), E = B(async () => {
    try {
      d(null);
      let w;
      if (n)
        w = await n(c);
      else
        try {
          const A = await fetch(`${e}?timeFrame=${c}`);
          if (A.ok) {
            const D = await A.json();
            w = D.metrics || D;
          } else
            w = Ys(c);
        } catch {
          w = Ys(c);
        }
      const S = /* @__PURE__ */ new Date();
      a({
        metrics: w,
        lastUpdated: S,
        timeFrame: c
      }), b(S);
    } catch (w) {
      d(w instanceof Error ? w.message : "خطا در دریافت داده‌ها");
    } finally {
      l(!1);
    }
  }, [e, c, n]), N = B(async () => {
    l(!0), await E();
  }, [E]), _ = B(() => {
    m((w) => !w);
  }, []);
  return me(() => {
    E();
  }, [E]), me(() => {
    if (y || r <= 0) {
      g.current && (clearInterval(g.current), g.current = null);
      return;
    }
    return g.current = setInterval(E, r), () => {
      g.current && clearInterval(g.current);
    };
  }, [y, r, E]), {
    data: s,
    isLoading: i,
    error: u,
    timeFrame: c,
    setTimeFrame: f,
    isPaused: y,
    togglePause: _,
    refresh: N,
    lastUpdated: p
  };
}, nu = ({ isPaused: e, lastUpdated: t, onToggle: r }) => {
  const n = (s) => s ? s.toLocaleTimeString("fa-IR", {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit"
  }) : "—";
  return /* @__PURE__ */ o.jsxs("div", { className: "flex items-center gap-3", children: [
    /* @__PURE__ */ o.jsxs("div", { className: "flex items-center gap-2 text-sm text-gray-500 dark:text-gray-400", children: [
      /* @__PURE__ */ o.jsx(
        "span",
        {
          className: j(
            "w-2 h-2 rounded-full",
            e ? "bg-gray-400" : "bg-emerald-500 animate-pulse"
          )
        }
      ),
      /* @__PURE__ */ o.jsxs("span", { children: [
        "آخرین بروزرسانی: ",
        n(t)
      ] })
    ] }),
    /* @__PURE__ */ o.jsx(
      "button",
      {
        onClick: r,
        className: j(
          "px-3 py-1.5 text-sm font-medium rounded-lg",
          "transition-colors duration-200",
          e ? "bg-emerald-100 text-emerald-700 hover:bg-emerald-200 dark:bg-emerald-900/30 dark:text-emerald-400" : "bg-gray-100 text-gray-700 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-300"
        ),
        children: e ? /* @__PURE__ */ o.jsxs("span", { className: "flex items-center gap-1.5", children: [
          /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { d: "M8 5v14l11-7z" }) }),
          "ادامه"
        ] }) : /* @__PURE__ */ o.jsxs("span", { className: "flex items-center gap-1.5", children: [
          /* @__PURE__ */ o.jsx("svg", { className: "w-4 h-4", fill: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { d: "M6 4h4v16H6V4zm8 0h4v16h-4V4z" }) }),
          "توقف"
        ] })
      }
    )
  ] });
}, su = ({
  apiUrl: e,
  initialTimeFrame: t = "5m",
  refreshInterval: r = 5e3,
  fetchFn: n,
  onMetricClick: s,
  groupByCategory: a = !0,
  title: i = "عملیات لایو",
  subtitle: l = "نمایش آنی داده‌های عملیاتی کسب‌وکار",
  headerExtra: u,
  className: d
}) => {
  const {
    data: c,
    isLoading: f,
    error: y,
    timeFrame: m,
    setTimeFrame: p,
    isPaused: b,
    togglePause: g,
    refresh: E,
    lastUpdated: N
  } = ru({
    apiUrl: e,
    initialTimeFrame: t,
    refreshInterval: r,
    fetchFn: n
  }), _ = ke(() => {
    if (!(c != null && c.metrics) || !a)
      return { ungrouped: (c == null ? void 0 : c.metrics) || [] };
    const w = {};
    return c.metrics.forEach((S) => {
      const A = S.category || "سایر";
      w[A] || (w[A] = []), w[A].push(S);
    }), w;
  }, [c == null ? void 0 : c.metrics, a]);
  return y ? /* @__PURE__ */ o.jsx("div", { className: j("bg-white dark:bg-gray-800 rounded-2xl p-8", d), children: /* @__PURE__ */ o.jsxs("div", { className: "text-center", children: [
    /* @__PURE__ */ o.jsx("div", { className: "w-16 h-16 bg-red-100 dark:bg-red-900/30 rounded-full flex items-center justify-center mx-auto mb-4", children: /* @__PURE__ */ o.jsx("svg", { className: "w-8 h-8 text-red-500", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" }) }) }),
    /* @__PURE__ */ o.jsx("h3", { className: "text-lg font-medium text-gray-900 dark:text-white mb-2", children: "خطا در دریافت داده‌ها" }),
    /* @__PURE__ */ o.jsx("p", { className: "text-gray-500 dark:text-gray-400 mb-4", children: y }),
    /* @__PURE__ */ o.jsx(
      "button",
      {
        onClick: E,
        className: "px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-500 transition-colors",
        children: "تلاش مجدد"
      }
    )
  ] }) }) : /* @__PURE__ */ o.jsxs("div", { className: j("space-y-6", d), children: [
    /* @__PURE__ */ o.jsxs("div", { className: "flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4", children: [
      /* @__PURE__ */ o.jsxs("div", { children: [
        /* @__PURE__ */ o.jsx("h1", { className: "text-2xl font-bold text-gray-900 dark:text-white", children: i }),
        /* @__PURE__ */ o.jsx("p", { className: "text-sm text-gray-500 dark:text-gray-400 mt-1", children: l })
      ] }),
      /* @__PURE__ */ o.jsxs("div", { className: "flex flex-wrap items-center gap-4", children: [
        u,
        /* @__PURE__ */ o.jsx(
          Xd,
          {
            value: m,
            onChange: p,
            size: "md"
          }
        )
      ] })
    ] }),
    /* @__PURE__ */ o.jsxs("div", { className: "flex items-center justify-between", children: [
      /* @__PURE__ */ o.jsx(
        nu,
        {
          isPaused: b,
          lastUpdated: N,
          onToggle: g
        }
      ),
      /* @__PURE__ */ o.jsx(
        "button",
        {
          onClick: E,
          disabled: f,
          className: j(
            "p-2 rounded-lg text-gray-500 hover:text-gray-700 hover:bg-gray-100",
            "dark:text-gray-400 dark:hover:text-gray-200 dark:hover:bg-gray-700",
            "transition-colors duration-200",
            f && "animate-spin"
          ),
          children: /* @__PURE__ */ o.jsx("svg", { className: "w-5 h-5", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" }) })
        }
      )
    ] }),
    a ? Object.entries(_).map(([w, S]) => /* @__PURE__ */ o.jsxs("div", { children: [
      w !== "ungrouped" && /* @__PURE__ */ o.jsx("h2", { className: "text-lg font-semibold text-gray-700 dark:text-gray-300 mb-4", children: w }),
      /* @__PURE__ */ o.jsx("div", { className: "grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4", children: S.map((A) => /* @__PURE__ */ o.jsx(
        dn,
        {
          metric: A,
          loading: f && !c,
          onClick: s ? () => s(A) : void 0
        },
        A.id
      )) })
    ] }, w)) : /* @__PURE__ */ o.jsxs("div", { className: "grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4", children: [
      ((c == null ? void 0 : c.metrics) || []).map((w) => /* @__PURE__ */ o.jsx(
        dn,
        {
          metric: w,
          loading: f && !c,
          onClick: s ? () => s(w) : void 0
        },
        w.id
      )),
      f && !c && Array.from({ length: 6 }).map((w, S) => /* @__PURE__ */ o.jsx(
        dn,
        {
          metric: {
            id: `skeleton-${S}`,
            name: "",
            value: 0
          },
          loading: !0
        },
        `skeleton-${S}`
      ))
    ] }),
    !f && (c == null ? void 0 : c.metrics.length) === 0 && /* @__PURE__ */ o.jsxs("div", { className: "bg-white dark:bg-gray-800 rounded-2xl p-12 text-center", children: [
      /* @__PURE__ */ o.jsx("div", { className: "w-16 h-16 bg-gray-100 dark:bg-gray-700 rounded-full flex items-center justify-center mx-auto mb-4", children: /* @__PURE__ */ o.jsx("svg", { className: "w-8 h-8 text-gray-400", fill: "none", stroke: "currentColor", viewBox: "0 0 24 24", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" }) }) }),
      /* @__PURE__ */ o.jsx("h3", { className: "text-lg font-medium text-gray-900 dark:text-white mb-2", children: "داده‌ای یافت نشد" }),
      /* @__PURE__ */ o.jsx("p", { className: "text-gray-500 dark:text-gray-400", children: "در این بازه زمانی داده‌ای برای نمایش وجود ندارد" })
    ] })
  ] });
}, Rt = (e, t) => t === "currency" ? new Intl.NumberFormat("fa-IR").format(e) + " ریال" : t === "percent" ? e.toFixed(1) + "%" : new Intl.NumberFormat("fa-IR").format(e), th = ({
  apiUrl: e,
  refreshInterval: t = 5e3
}) => {
  const [r, n] = G(null), s = (i) => {
    n(i);
  }, a = () => {
    n(null);
  };
  return /* @__PURE__ */ o.jsxs("div", { className: "space-y-6", children: [
    /* @__PURE__ */ o.jsx(
      su,
      {
        apiUrl: e,
        refreshInterval: t,
        onMetricClick: s,
        groupByCategory: !0
      }
    ),
    /* @__PURE__ */ o.jsx(
      Ci,
      {
        isOpen: !!r,
        onClose: a,
        title: (r == null ? void 0 : r.name) || "",
        description: r == null ? void 0 : r.nameEn,
        size: "lg",
        children: r && /* @__PURE__ */ o.jsxs("div", { className: "space-y-6", children: [
          /* @__PURE__ */ o.jsxs("div", { className: "text-center py-6 bg-gray-50 dark:bg-gray-900 rounded-xl", children: [
            /* @__PURE__ */ o.jsx("div", { className: "text-4xl font-bold text-gray-900 dark:text-white mb-2", children: Rt(r.value, r.format) }),
            r.unit && /* @__PURE__ */ o.jsx("div", { className: "text-gray-500", children: r.unit }),
            r.trend && /* @__PURE__ */ o.jsxs(
              "div",
              {
                className: `mt-2 inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm ${r.trend === "up" ? "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400" : r.trend === "down" ? "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400" : "bg-gray-100 text-gray-600 dark:bg-gray-700 dark:text-gray-400"}`,
                children: [
                  r.trend === "up" ? "↑" : r.trend === "down" ? "↓" : "—",
                  r.trendPercent !== void 0 && /* @__PURE__ */ o.jsxs("span", { children: [
                    r.trendPercent > 0 ? "+" : "",
                    r.trendPercent.toFixed(1),
                    "%"
                  ] })
                ]
              }
            )
          ] }),
          r.sparklineData && r.sparklineData.length > 0 && /* @__PURE__ */ o.jsxs("div", { children: [
            /* @__PURE__ */ o.jsx("h4", { className: "text-sm font-medium text-gray-700 dark:text-gray-300 mb-3", children: "روند تغییرات" }),
            /* @__PURE__ */ o.jsx("div", { className: "bg-gray-50 dark:bg-gray-900 rounded-xl p-4", children: /* @__PURE__ */ o.jsx(
              ja,
              {
                data: r.sparklineData,
                width: 400,
                height: 120,
                color: r.color || "primary",
                strokeWidth: 3
              }
            ) })
          ] }),
          r.sparklineData && r.sparklineData.length > 0 && /* @__PURE__ */ o.jsxs("div", { children: [
            /* @__PURE__ */ o.jsx("h4", { className: "text-sm font-medium text-gray-700 dark:text-gray-300 mb-3", children: "آمار" }),
            /* @__PURE__ */ o.jsxs("div", { className: "grid grid-cols-2 sm:grid-cols-4 gap-4", children: [
              /* @__PURE__ */ o.jsxs("div", { className: "bg-gray-50 dark:bg-gray-900 rounded-xl p-4 text-center", children: [
                /* @__PURE__ */ o.jsx("div", { className: "text-xs text-gray-500 mb-1", children: "حداقل" }),
                /* @__PURE__ */ o.jsx("div", { className: "text-lg font-semibold text-gray-900 dark:text-white", children: Rt(Math.min(...r.sparklineData), r.format) })
              ] }),
              /* @__PURE__ */ o.jsxs("div", { className: "bg-gray-50 dark:bg-gray-900 rounded-xl p-4 text-center", children: [
                /* @__PURE__ */ o.jsx("div", { className: "text-xs text-gray-500 mb-1", children: "حداکثر" }),
                /* @__PURE__ */ o.jsx("div", { className: "text-lg font-semibold text-gray-900 dark:text-white", children: Rt(Math.max(...r.sparklineData), r.format) })
              ] }),
              /* @__PURE__ */ o.jsxs("div", { className: "bg-gray-50 dark:bg-gray-900 rounded-xl p-4 text-center", children: [
                /* @__PURE__ */ o.jsx("div", { className: "text-xs text-gray-500 mb-1", children: "میانگین" }),
                /* @__PURE__ */ o.jsx("div", { className: "text-lg font-semibold text-gray-900 dark:text-white", children: Rt(
                  r.sparklineData.reduce((i, l) => i + l, 0) / r.sparklineData.length,
                  r.format
                ) })
              ] }),
              /* @__PURE__ */ o.jsxs("div", { className: "bg-gray-50 dark:bg-gray-900 rounded-xl p-4 text-center", children: [
                /* @__PURE__ */ o.jsx("div", { className: "text-xs text-gray-500 mb-1", children: "آخرین" }),
                /* @__PURE__ */ o.jsx("div", { className: "text-lg font-semibold text-gray-900 dark:text-white", children: Rt(
                  r.sparklineData[r.sparklineData.length - 1],
                  r.format
                ) })
              ] })
            ] })
          ] }),
          /* @__PURE__ */ o.jsx("div", { className: "border-t border-gray-200 dark:border-gray-700 pt-4", children: /* @__PURE__ */ o.jsxs("div", { className: "grid grid-cols-2 gap-4 text-sm", children: [
            /* @__PURE__ */ o.jsxs("div", { children: [
              /* @__PURE__ */ o.jsx("span", { className: "text-gray-500", children: "شناسه:" }),
              /* @__PURE__ */ o.jsx("span", { className: "mr-2 text-gray-900 dark:text-white font-mono", children: r.id })
            ] }),
            r.category && /* @__PURE__ */ o.jsxs("div", { children: [
              /* @__PURE__ */ o.jsx("span", { className: "text-gray-500", children: "دسته‌بندی:" }),
              /* @__PURE__ */ o.jsx("span", { className: "mr-2 text-gray-900 dark:text-white", children: r.category })
            ] })
          ] }) })
        ] })
      }
    )
  ] });
};
function Fr() {
  const e = ka(), t = xe(Vn), r = xe(Jo), n = xe(Qo), s = B((d) => d.some((c) => e.hasPermission(c)), [e]), a = B((d) => d.every((c) => e.hasPermission(c)), [e]), i = B((d) => d.some((c) => e.hasRole(c)), [e]), l = B((d) => d.every((c) => e.hasRole(c)), [e]), u = ke(() => ({
    isAdmin: (t == null ? void 0 : t.isAdmin) ?? !1,
    displayName: (t == null ? void 0 : t.displayName) ?? (t == null ? void 0 : t.username) ?? "",
    permissions: (t == null ? void 0 : t.permissions) ?? [],
    roles: (t == null ? void 0 : t.roles) ?? []
  }), [t]);
  return {
    // State from context
    user: e.user,
    isAuthenticated: e.isAuthenticated,
    isLoading: e.isLoading,
    error: e.error,
    accessToken: r,
    expiresAt: n,
    // Actions from context
    login: e.login,
    logout: e.logout,
    refreshToken: e.refreshToken,
    updateUser: e.updateUser,
    // Permission helpers
    hasPermission: e.hasPermission,
    hasRole: e.hasRole,
    hasAnyPermission: s,
    hasAllPermissions: a,
    hasAnyRole: i,
    hasAllRoles: l,
    // User helpers
    ...u
  };
}
function rh() {
  return xe(Ho);
}
function nh() {
  return xe(Vn);
}
function sh() {
  return xe(qo);
}
function oh() {
  return xe(Ko);
}
function ah(e) {
  const { hasPermission: t } = Fr();
  return t(e);
}
function ih(e) {
  const { hasRole: t } = Fr();
  return t(e);
}
function lh() {
  const e = Fr();
  if (!e.isAuthenticated && !e.isLoading)
    throw new Error("User is not authenticated");
  return e;
}
function ou() {
  return /* @__PURE__ */ o.jsx("div", { className: "flex items-center justify-center min-h-screen bg-slate-100 dark:bg-slate-900", children: /* @__PURE__ */ o.jsxs("div", { className: "flex flex-col items-center gap-4", children: [
    /* @__PURE__ */ o.jsx("div", { className: "w-12 h-12 border-4 border-primary-500 border-t-transparent rounded-full animate-spin" }),
    /* @__PURE__ */ o.jsx("p", { className: "text-slate-600 dark:text-slate-400", children: "در حال بارگذاری..." })
  ] }) });
}
function au() {
  return /* @__PURE__ */ o.jsx("div", { className: "flex items-center justify-center min-h-screen bg-slate-100 dark:bg-slate-900", children: /* @__PURE__ */ o.jsxs("div", { className: "text-center p-8 bg-white dark:bg-slate-800 rounded-xl shadow-lg max-w-md", children: [
    /* @__PURE__ */ o.jsx("div", { className: "w-16 h-16 mx-auto mb-4 bg-red-100 dark:bg-red-900/30 rounded-full flex items-center justify-center", children: /* @__PURE__ */ o.jsx("svg", { className: "w-8 h-8 text-red-600 dark:text-red-400", fill: "none", viewBox: "0 0 24 24", stroke: "currentColor", children: /* @__PURE__ */ o.jsx("path", { strokeLinecap: "round", strokeLinejoin: "round", strokeWidth: 2, d: "M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" }) }) }),
    /* @__PURE__ */ o.jsx("h2", { className: "text-xl font-bold text-slate-800 dark:text-white mb-2", children: "دسترسی غیرمجاز" }),
    /* @__PURE__ */ o.jsx("p", { className: "text-slate-600 dark:text-slate-400 mb-4", children: "شما مجوز دسترسی به این صفحه را ندارید." }),
    /* @__PURE__ */ o.jsx(
      "a",
      {
        href: "/",
        className: "inline-block px-4 py-2 bg-primary-500 text-white rounded-lg hover:bg-primary-600 transition-colors",
        children: "بازگشت به صفحه اصلی"
      }
    )
  ] }) });
}
function Sa({
  children: e,
  requiredPermissions: t = [],
  requiredRoles: r = [],
  requireAll: n = !1,
  fallback: s,
  redirectTo: a = "/login"
}) {
  const {
    isAuthenticated: i,
    isLoading: l,
    hasAllPermissions: u,
    hasAnyPermission: d,
    hasAllRoles: c,
    hasAnyRole: f
  } = Fr(), y = Fn(), m = ke(() => {
    if (t.length === 0 && r.length === 0)
      return !0;
    const p = t.length === 0 || (n ? u(t) : d(t)), b = r.length === 0 || (n ? c(r) : f(r));
    return t.length > 0 && r.length > 0 ? n ? p && b : p || b : p && b;
  }, [
    t,
    r,
    n,
    u,
    d,
    c,
    f
  ]);
  return l ? s ? /* @__PURE__ */ o.jsx(o.Fragment, { children: s }) : /* @__PURE__ */ o.jsx(ou, {}) : i ? m ? /* @__PURE__ */ o.jsx(o.Fragment, { children: e }) : s ? /* @__PURE__ */ o.jsx(o.Fragment, { children: s }) : /* @__PURE__ */ o.jsx(au, {}) : /* @__PURE__ */ o.jsx(
    ai,
    {
      to: a,
      state: { from: y.pathname + y.search },
      replace: !0
    }
  );
}
const iu = In(() => Promise.resolve().then(() => Hd)), lu = In(() => Promise.resolve().then(() => Yd)), cu = In(() => Promise.resolve().then(() => Gd));
function _a() {
  return /* @__PURE__ */ o.jsx("div", { className: "min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900", children: /* @__PURE__ */ o.jsxs("div", { className: "text-center", children: [
    /* @__PURE__ */ o.jsx("div", { className: "w-12 h-12 border-4 border-purple-500 border-t-transparent rounded-full animate-spin mx-auto mb-4" }),
    /* @__PURE__ */ o.jsx("p", { className: "text-gray-500 dark:text-gray-400", children: "در حال بارگذاری..." })
  ] }) });
}
function du({ menuItems: e, logo: t, appTitle: r, headerActions: n }) {
  return /* @__PURE__ */ o.jsx(
    yi,
    {
      menuItems: e,
      logo: t,
      logoText: r,
      headerActions: n,
      children: /* @__PURE__ */ o.jsx(mo, { fallback: /* @__PURE__ */ o.jsx(_a, {}), children: /* @__PURE__ */ o.jsx(ci, {}) })
    }
  );
}
function Ra(e) {
  return e.map((t, r) => {
    const n = t.permission || t.role ? /* @__PURE__ */ o.jsx(
      Sa,
      {
        requiredPermissions: t.permission ? Array.isArray(t.permission) ? t.permission : [t.permission] : void 0,
        requiredRoles: t.role ? Array.isArray(t.role) ? t.role : [t.role] : void 0,
        children: t.element
      }
    ) : t.element;
    return t.children ? /* @__PURE__ */ o.jsx(Qe, { path: t.path, element: n, children: Ra(t.children) }, r) : t.index ? /* @__PURE__ */ o.jsx(Qe, { index: !0, element: n }, r) : /* @__PURE__ */ o.jsx(Qe, { path: t.path, element: n }, r);
  });
}
function ch({
  menuItems: e = [],
  routes: t = [],
  loginPage: r,
  dashboardPage: n,
  notFoundPage: s,
  authConfig: a,
  basePath: i = "/",
  logo: l,
  appTitle: u = "پنل مدیریت",
  loadingFallback: d,
  onAuthStateChange: c,
  onSessionExpired: f,
  headerActions: y
}) {
  const m = d || /* @__PURE__ */ o.jsx(_a, {});
  return /* @__PURE__ */ o.jsx(ii, { basename: i, children: /* @__PURE__ */ o.jsx(
    Wd,
    {
      config: a,
      onAuthStateChange: c,
      onSessionExpired: f,
      children: /* @__PURE__ */ o.jsx(mo, { fallback: m, children: /* @__PURE__ */ o.jsxs(li, { children: [
        /* @__PURE__ */ o.jsx(
          Qe,
          {
            path: "/login",
            element: r || /* @__PURE__ */ o.jsx(iu, { logo: l, title: u })
          }
        ),
        /* @__PURE__ */ o.jsxs(
          Qe,
          {
            element: /* @__PURE__ */ o.jsx(Sa, { redirectTo: "/login", children: /* @__PURE__ */ o.jsx(
              du,
              {
                menuItems: e,
                logo: l,
                appTitle: u,
                headerActions: y
              }
            ) }),
            children: [
              /* @__PURE__ */ o.jsx(
                Qe,
                {
                  index: !0,
                  element: n || /* @__PURE__ */ o.jsx(cu, {})
                }
              ),
              Ra(t)
            ]
          }
        ),
        /* @__PURE__ */ o.jsx(
          Qe,
          {
            path: "*",
            element: s || /* @__PURE__ */ o.jsx(lu, {})
          }
        )
      ] }) })
    }
  ) });
}
var Ur = class {
  constructor() {
    this.listeners = /* @__PURE__ */ new Set(), this.subscribe = this.subscribe.bind(this);
  }
  subscribe(e) {
    return this.listeners.add(e), this.onSubscribe(), () => {
      this.listeners.delete(e), this.onUnsubscribe();
    };
  }
  hasListeners() {
    return this.listeners.size > 0;
  }
  onSubscribe() {
  }
  onUnsubscribe() {
  }
}, uu = {
  // We need the wrapper function syntax below instead of direct references to
  // global setTimeout etc.
  //
  // BAD: `setTimeout: setTimeout`
  // GOOD: `setTimeout: (cb, delay) => setTimeout(cb, delay)`
  //
  // If we use direct references here, then anything that wants to spy on or
  // replace the global setTimeout (like tests) won't work since we'll already
  // have a hard reference to the original implementation at the time when this
  // file was imported.
  setTimeout: (e, t) => setTimeout(e, t),
  clearTimeout: (e) => clearTimeout(e),
  setInterval: (e, t) => setInterval(e, t),
  clearInterval: (e) => clearInterval(e)
}, Ae, Ye, io, fu = (io = class {
  constructor() {
    // We cannot have TimeoutManager<T> as we must instantiate it with a concrete
    // type at app boot; and if we leave that type, then any new timer provider
    // would need to support ReturnType<typeof setTimeout>, which is infeasible.
    //
    // We settle for type safety for the TimeoutProvider type, and accept that
    // this class is unsafe internally to allow for extension.
    K(this, Ae, uu);
    K(this, Ye, !1);
  }
  setTimeoutProvider(e) {
    process.env.NODE_ENV !== "production" && v(this, Ye) && e !== v(this, Ae) && console.error(
      "[timeoutManager]: Switching provider after calls to previous provider might result in unexpected behavior.",
      { previous: v(this, Ae), provider: e }
    ), I(this, Ae, e), process.env.NODE_ENV !== "production" && I(this, Ye, !1);
  }
  setTimeout(e, t) {
    return process.env.NODE_ENV !== "production" && I(this, Ye, !0), v(this, Ae).setTimeout(e, t);
  }
  clearTimeout(e) {
    v(this, Ae).clearTimeout(e);
  }
  setInterval(e, t) {
    return process.env.NODE_ENV !== "production" && I(this, Ye, !0), v(this, Ae).setInterval(e, t);
  }
  clearInterval(e) {
    v(this, Ae).clearInterval(e);
  }
}, Ae = new WeakMap(), Ye = new WeakMap(), io), sr = new fu();
function hu(e) {
  setTimeout(e, 0);
}
var Ft = typeof window > "u" || "Deno" in globalThis;
function Er() {
}
function Xs(e) {
  return typeof e == "number" && e >= 0 && e !== 1 / 0;
}
function pu(e, t) {
  return Math.max(e + (t || 0) - Date.now(), 0);
}
function Mt(e, t) {
  return typeof e == "function" ? e(t) : e;
}
function Se(e, t) {
  return typeof e == "function" ? e(t) : e;
}
function Zs(e) {
  return JSON.stringify(
    e,
    (t, r) => Sn(r) ? Object.keys(r).sort().reduce((n, s) => (n[s] = r[s], n), {}) : r
  );
}
var mu = Object.prototype.hasOwnProperty;
function jn(e, t) {
  if (e === t)
    return e;
  const r = eo(e) && eo(t);
  if (!r && !(Sn(e) && Sn(t))) return t;
  const s = (r ? e : Object.keys(e)).length, a = r ? t : Object.keys(t), i = a.length, l = r ? new Array(i) : {};
  let u = 0;
  for (let d = 0; d < i; d++) {
    const c = r ? d : a[d], f = e[c], y = t[c];
    if (f === y) {
      l[c] = f, (r ? d < s : mu.call(e, c)) && u++;
      continue;
    }
    if (f === null || y === null || typeof f != "object" || typeof y != "object") {
      l[c] = y;
      continue;
    }
    const m = jn(f, y);
    l[c] = m, m === f && u++;
  }
  return s === i && u === s ? e : l;
}
function kr(e, t) {
  if (!t || Object.keys(e).length !== Object.keys(t).length)
    return !1;
  for (const r in e)
    if (e[r] !== t[r])
      return !1;
  return !0;
}
function eo(e) {
  return Array.isArray(e) && e.length === Object.keys(e).length;
}
function Sn(e) {
  if (!to(e))
    return !1;
  const t = e.constructor;
  if (t === void 0)
    return !0;
  const r = t.prototype;
  return !(!to(r) || !r.hasOwnProperty("isPrototypeOf") || Object.getPrototypeOf(e) !== Object.prototype);
}
function to(e) {
  return Object.prototype.toString.call(e) === "[object Object]";
}
function ro(e, t, r) {
  if (typeof r.structuralSharing == "function")
    return r.structuralSharing(e, t);
  if (r.structuralSharing !== !1) {
    if (process.env.NODE_ENV !== "production")
      try {
        return jn(e, t);
      } catch (n) {
        throw console.error(
          `Structural sharing requires data to be JSON serializable. To fix this, turn off structuralSharing or return JSON-serializable data from your queryFn. [${r.queryHash}]: ${n}`
        ), n;
      }
    return jn(e, t);
  }
  return t;
}
function Oa(e, t) {
  return typeof e == "function" ? e(...t) : !!e;
}
var Xe, Ve, dt, lo, gu = (lo = class extends Ur {
  constructor() {
    super();
    K(this, Xe);
    K(this, Ve);
    K(this, dt);
    I(this, dt, (t) => {
      if (!Ft && window.addEventListener) {
        const r = () => t();
        return window.addEventListener("visibilitychange", r, !1), () => {
          window.removeEventListener("visibilitychange", r);
        };
      }
    });
  }
  onSubscribe() {
    v(this, Ve) || this.setEventListener(v(this, dt));
  }
  onUnsubscribe() {
    var t;
    this.hasListeners() || ((t = v(this, Ve)) == null || t.call(this), I(this, Ve, void 0));
  }
  setEventListener(t) {
    var r;
    I(this, dt, t), (r = v(this, Ve)) == null || r.call(this), I(this, Ve, t((n) => {
      typeof n == "boolean" ? this.setFocused(n) : this.onFocus();
    }));
  }
  setFocused(t) {
    v(this, Xe) !== t && (I(this, Xe, t), this.onFocus());
  }
  onFocus() {
    const t = this.isFocused();
    this.listeners.forEach((r) => {
      r(t);
    });
  }
  isFocused() {
    var t;
    return typeof v(this, Xe) == "boolean" ? v(this, Xe) : ((t = globalThis.document) == null ? void 0 : t.visibilityState) !== "hidden";
  }
}, Xe = new WeakMap(), Ve = new WeakMap(), dt = new WeakMap(), lo), yu = new gu();
function no() {
  let e, t;
  const r = new Promise((s, a) => {
    e = s, t = a;
  });
  r.status = "pending", r.catch(() => {
  });
  function n(s) {
    Object.assign(r, s), delete r.resolve, delete r.reject;
  }
  return r.resolve = (s) => {
    n({
      status: "fulfilled",
      value: s
    }), e(s);
  }, r.reject = (s) => {
    n({
      status: "rejected",
      reason: s
    }), t(s);
  }, r;
}
var xu = hu;
function bu() {
  let e = [], t = 0, r = (l) => {
    l();
  }, n = (l) => {
    l();
  }, s = xu;
  const a = (l) => {
    t ? e.push(l) : s(() => {
      r(l);
    });
  }, i = () => {
    const l = e;
    e = [], l.length && s(() => {
      n(() => {
        l.forEach((u) => {
          r(u);
        });
      });
    });
  };
  return {
    batch: (l) => {
      let u;
      t++;
      try {
        u = l();
      } finally {
        t--, t || i();
      }
      return u;
    },
    /**
     * All calls to the wrapped function will be batched.
     */
    batchCalls: (l) => (...u) => {
      a(() => {
        l(...u);
      });
    },
    schedule: a,
    /**
     * Use this method to set a custom notify function.
     * This can be used to for example wrap notifications with `React.act` while running tests.
     */
    setNotifyFunction: (l) => {
      r = l;
    },
    /**
     * Use this method to set a custom function to batch notifications together into a single tick.
     * By default React Query will use the batch function provided by ReactDOM or React Native.
     */
    setBatchNotifyFunction: (l) => {
      n = l;
    },
    setScheduler: (l) => {
      s = l;
    }
  };
}
var $r = bu(), ut, ze, ft, co, vu = (co = class extends Ur {
  constructor() {
    super();
    K(this, ut, !0);
    K(this, ze);
    K(this, ft);
    I(this, ft, (t) => {
      if (!Ft && window.addEventListener) {
        const r = () => t(!0), n = () => t(!1);
        return window.addEventListener("online", r, !1), window.addEventListener("offline", n, !1), () => {
          window.removeEventListener("online", r), window.removeEventListener("offline", n);
        };
      }
    });
  }
  onSubscribe() {
    v(this, ze) || this.setEventListener(v(this, ft));
  }
  onUnsubscribe() {
    var t;
    this.hasListeners() || ((t = v(this, ze)) == null || t.call(this), I(this, ze, void 0));
  }
  setEventListener(t) {
    var r;
    I(this, ft, t), (r = v(this, ze)) == null || r.call(this), I(this, ze, t(this.setOnline.bind(this)));
  }
  setOnline(t) {
    v(this, ut) !== t && (I(this, ut, t), this.listeners.forEach((n) => {
      n(t);
    }));
  }
  isOnline() {
    return v(this, ut);
  }
}, ut = new WeakMap(), ze = new WeakMap(), ft = new WeakMap(), co), wu = new vu();
function Eu(e) {
  return (e ?? "online") === "online" ? wu.isOnline() : !0;
}
function ku(e, t) {
  return {
    fetchFailureCount: 0,
    fetchFailureReason: null,
    fetchStatus: Eu(t.networkMode) ? "fetching" : "paused",
    ...e === void 0 && {
      error: null,
      status: "pending"
    }
  };
}
var he, W, Ut, ce, Ze, ht, Pe, We, $t, pt, mt, et, tt, He, gt, J, Ct, _n, Rn, On, Tn, Cn, An, Dn, Ta, uo, Nu = (uo = class extends Ur {
  constructor(t, r) {
    super();
    K(this, J);
    K(this, he);
    K(this, W);
    K(this, Ut);
    K(this, ce);
    K(this, Ze);
    K(this, ht);
    K(this, Pe);
    K(this, We);
    K(this, $t);
    K(this, pt);
    // This property keeps track of the last query with defined data.
    // It will be used to pass the previous data and query to the placeholder function between renders.
    K(this, mt);
    K(this, et);
    K(this, tt);
    K(this, He);
    K(this, gt, /* @__PURE__ */ new Set());
    this.options = r, I(this, he, t), I(this, We, null), I(this, Pe, no()), this.bindMethods(), this.setOptions(r);
  }
  bindMethods() {
    this.refetch = this.refetch.bind(this);
  }
  onSubscribe() {
    this.listeners.size === 1 && (v(this, W).addObserver(this), so(v(this, W), this.options) ? ee(this, J, Ct).call(this) : this.updateResult(), ee(this, J, Tn).call(this));
  }
  onUnsubscribe() {
    this.hasListeners() || this.destroy();
  }
  shouldFetchOnReconnect() {
    return Mn(
      v(this, W),
      this.options,
      this.options.refetchOnReconnect
    );
  }
  shouldFetchOnWindowFocus() {
    return Mn(
      v(this, W),
      this.options,
      this.options.refetchOnWindowFocus
    );
  }
  destroy() {
    this.listeners = /* @__PURE__ */ new Set(), ee(this, J, Cn).call(this), ee(this, J, An).call(this), v(this, W).removeObserver(this);
  }
  setOptions(t) {
    const r = this.options, n = v(this, W);
    if (this.options = v(this, he).defaultQueryOptions(t), this.options.enabled !== void 0 && typeof this.options.enabled != "boolean" && typeof this.options.enabled != "function" && typeof Se(this.options.enabled, v(this, W)) != "boolean")
      throw new Error(
        "Expected enabled to be a boolean or a callback that returns a boolean"
      );
    ee(this, J, Dn).call(this), v(this, W).setOptions(this.options), r._defaulted && !kr(this.options, r) && v(this, he).getQueryCache().notify({
      type: "observerOptionsUpdated",
      query: v(this, W),
      observer: this
    });
    const s = this.hasListeners();
    s && oo(
      v(this, W),
      n,
      this.options,
      r
    ) && ee(this, J, Ct).call(this), this.updateResult(), s && (v(this, W) !== n || Se(this.options.enabled, v(this, W)) !== Se(r.enabled, v(this, W)) || Mt(this.options.staleTime, v(this, W)) !== Mt(r.staleTime, v(this, W))) && ee(this, J, _n).call(this);
    const a = ee(this, J, Rn).call(this);
    s && (v(this, W) !== n || Se(this.options.enabled, v(this, W)) !== Se(r.enabled, v(this, W)) || a !== v(this, He)) && ee(this, J, On).call(this, a);
  }
  getOptimisticResult(t) {
    const r = v(this, he).getQueryCache().build(v(this, he), t), n = this.createResult(r, t);
    return Su(this, n) && (I(this, ce, n), I(this, ht, this.options), I(this, Ze, v(this, W).state)), n;
  }
  getCurrentResult() {
    return v(this, ce);
  }
  trackResult(t, r) {
    return new Proxy(t, {
      get: (n, s) => (this.trackProp(s), r == null || r(s), s === "promise" && (this.trackProp("data"), !this.options.experimental_prefetchInRender && v(this, Pe).status === "pending" && v(this, Pe).reject(
        new Error(
          "experimental_prefetchInRender feature flag is not enabled"
        )
      )), Reflect.get(n, s))
    });
  }
  trackProp(t) {
    v(this, gt).add(t);
  }
  getCurrentQuery() {
    return v(this, W);
  }
  refetch({ ...t } = {}) {
    return this.fetch({
      ...t
    });
  }
  fetchOptimistic(t) {
    const r = v(this, he).defaultQueryOptions(t), n = v(this, he).getQueryCache().build(v(this, he), r);
    return n.fetch().then(() => this.createResult(n, r));
  }
  fetch(t) {
    return ee(this, J, Ct).call(this, {
      ...t,
      cancelRefetch: t.cancelRefetch ?? !0
    }).then(() => (this.updateResult(), v(this, ce)));
  }
  createResult(t, r) {
    var T;
    const n = v(this, W), s = this.options, a = v(this, ce), i = v(this, Ze), l = v(this, ht), d = t !== n ? t.state : v(this, Ut), { state: c } = t;
    let f = { ...c }, y = !1, m;
    if (r._optimisticResults) {
      const M = this.hasListeners(), C = !M && so(t, r), $ = M && oo(t, n, r, s);
      (C || $) && (f = {
        ...f,
        ...ku(c.data, t.options)
      }), r._optimisticResults === "isRestoring" && (f.fetchStatus = "idle");
    }
    let { error: p, errorUpdatedAt: b, status: g } = f;
    m = f.data;
    let E = !1;
    if (r.placeholderData !== void 0 && m === void 0 && g === "pending") {
      let M;
      a != null && a.isPlaceholderData && r.placeholderData === (l == null ? void 0 : l.placeholderData) ? (M = a.data, E = !0) : M = typeof r.placeholderData == "function" ? r.placeholderData(
        (T = v(this, mt)) == null ? void 0 : T.state.data,
        v(this, mt)
      ) : r.placeholderData, M !== void 0 && (g = "success", m = ro(
        a == null ? void 0 : a.data,
        M,
        r
      ), y = !0);
    }
    if (r.select && m !== void 0 && !E)
      if (a && m === (i == null ? void 0 : i.data) && r.select === v(this, $t))
        m = v(this, pt);
      else
        try {
          I(this, $t, r.select), m = r.select(m), m = ro(a == null ? void 0 : a.data, m, r), I(this, pt, m), I(this, We, null);
        } catch (M) {
          I(this, We, M);
        }
    v(this, We) && (p = v(this, We), m = v(this, pt), b = Date.now(), g = "error");
    const N = f.fetchStatus === "fetching", _ = g === "pending", w = g === "error", S = _ && N, A = m !== void 0, U = {
      status: g,
      fetchStatus: f.fetchStatus,
      isPending: _,
      isSuccess: g === "success",
      isError: w,
      isInitialLoading: S,
      isLoading: S,
      data: m,
      dataUpdatedAt: f.dataUpdatedAt,
      error: p,
      errorUpdatedAt: b,
      failureCount: f.fetchFailureCount,
      failureReason: f.fetchFailureReason,
      errorUpdateCount: f.errorUpdateCount,
      isFetched: f.dataUpdateCount > 0 || f.errorUpdateCount > 0,
      isFetchedAfterMount: f.dataUpdateCount > d.dataUpdateCount || f.errorUpdateCount > d.errorUpdateCount,
      isFetching: N,
      isRefetching: N && !_,
      isLoadingError: w && !A,
      isPaused: f.fetchStatus === "paused",
      isPlaceholderData: y,
      isRefetchError: w && A,
      isStale: Jn(t, r),
      refetch: this.refetch,
      promise: v(this, Pe),
      isEnabled: Se(r.enabled, t) !== !1
    };
    if (this.options.experimental_prefetchInRender) {
      const M = (R) => {
        U.status === "error" ? R.reject(U.error) : U.data !== void 0 && R.resolve(U.data);
      }, C = () => {
        const R = I(this, Pe, U.promise = no());
        M(R);
      }, $ = v(this, Pe);
      switch ($.status) {
        case "pending":
          t.queryHash === n.queryHash && M($);
          break;
        case "fulfilled":
          (U.status === "error" || U.data !== $.value) && C();
          break;
        case "rejected":
          (U.status !== "error" || U.error !== $.reason) && C();
          break;
      }
    }
    return U;
  }
  updateResult() {
    const t = v(this, ce), r = this.createResult(v(this, W), this.options);
    if (I(this, Ze, v(this, W).state), I(this, ht, this.options), v(this, Ze).data !== void 0 && I(this, mt, v(this, W)), kr(r, t))
      return;
    I(this, ce, r);
    const n = () => {
      if (!t)
        return !0;
      const { notifyOnChangeProps: s } = this.options, a = typeof s == "function" ? s() : s;
      if (a === "all" || !a && !v(this, gt).size)
        return !0;
      const i = new Set(
        a ?? v(this, gt)
      );
      return this.options.throwOnError && i.add("error"), Object.keys(v(this, ce)).some((l) => {
        const u = l;
        return v(this, ce)[u] !== t[u] && i.has(u);
      });
    };
    ee(this, J, Ta).call(this, { listeners: n() });
  }
  onQueryUpdate() {
    this.updateResult(), this.hasListeners() && ee(this, J, Tn).call(this);
  }
}, he = new WeakMap(), W = new WeakMap(), Ut = new WeakMap(), ce = new WeakMap(), Ze = new WeakMap(), ht = new WeakMap(), Pe = new WeakMap(), We = new WeakMap(), $t = new WeakMap(), pt = new WeakMap(), mt = new WeakMap(), et = new WeakMap(), tt = new WeakMap(), He = new WeakMap(), gt = new WeakMap(), J = new WeakSet(), Ct = function(t) {
  ee(this, J, Dn).call(this);
  let r = v(this, W).fetch(
    this.options,
    t
  );
  return t != null && t.throwOnError || (r = r.catch(Er)), r;
}, _n = function() {
  ee(this, J, Cn).call(this);
  const t = Mt(
    this.options.staleTime,
    v(this, W)
  );
  if (Ft || v(this, ce).isStale || !Xs(t))
    return;
  const n = pu(v(this, ce).dataUpdatedAt, t) + 1;
  I(this, et, sr.setTimeout(() => {
    v(this, ce).isStale || this.updateResult();
  }, n));
}, Rn = function() {
  return (typeof this.options.refetchInterval == "function" ? this.options.refetchInterval(v(this, W)) : this.options.refetchInterval) ?? !1;
}, On = function(t) {
  ee(this, J, An).call(this), I(this, He, t), !(Ft || Se(this.options.enabled, v(this, W)) === !1 || !Xs(v(this, He)) || v(this, He) === 0) && I(this, tt, sr.setInterval(() => {
    (this.options.refetchIntervalInBackground || yu.isFocused()) && ee(this, J, Ct).call(this);
  }, v(this, He)));
}, Tn = function() {
  ee(this, J, _n).call(this), ee(this, J, On).call(this, ee(this, J, Rn).call(this));
}, Cn = function() {
  v(this, et) && (sr.clearTimeout(v(this, et)), I(this, et, void 0));
}, An = function() {
  v(this, tt) && (sr.clearInterval(v(this, tt)), I(this, tt, void 0));
}, Dn = function() {
  const t = v(this, he).getQueryCache().build(v(this, he), this.options);
  if (t === v(this, W))
    return;
  const r = v(this, W);
  I(this, W, t), I(this, Ut, t.state), this.hasListeners() && (r == null || r.removeObserver(this), t.addObserver(this));
}, Ta = function(t) {
  $r.batch(() => {
    t.listeners && this.listeners.forEach((r) => {
      r(v(this, ce));
    }), v(this, he).getQueryCache().notify({
      query: v(this, W),
      type: "observerResultsUpdated"
    });
  });
}, uo);
function ju(e, t) {
  return Se(t.enabled, e) !== !1 && e.state.data === void 0 && !(e.state.status === "error" && t.retryOnMount === !1);
}
function so(e, t) {
  return ju(e, t) || e.state.data !== void 0 && Mn(e, t, t.refetchOnMount);
}
function Mn(e, t, r) {
  if (Se(t.enabled, e) !== !1 && Mt(t.staleTime, e) !== "static") {
    const n = typeof r == "function" ? r(e) : r;
    return n === "always" || n !== !1 && Jn(e, t);
  }
  return !1;
}
function oo(e, t, r, n) {
  return (e !== t || Se(n.enabled, e) === !1) && (!r.suspense || e.state.status !== "error") && Jn(e, r);
}
function Jn(e, t) {
  return Se(t.enabled, e) !== !1 && e.isStaleByTime(Mt(t.staleTime, e));
}
function Su(e, t) {
  return !kr(e.getCurrentResult(), t);
}
function _u() {
  return {
    context: void 0,
    data: void 0,
    error: null,
    failureCount: 0,
    failureReason: null,
    isPaused: !1,
    status: "idle",
    variables: void 0,
    submittedAt: 0
  };
}
var Le, qe, pe, Ie, Fe, ur, Pn, fo, Ru = (fo = class extends Ur {
  constructor(t, r) {
    super();
    K(this, Fe);
    K(this, Le);
    K(this, qe);
    K(this, pe);
    K(this, Ie);
    I(this, Le, t), this.setOptions(r), this.bindMethods(), ee(this, Fe, ur).call(this);
  }
  bindMethods() {
    this.mutate = this.mutate.bind(this), this.reset = this.reset.bind(this);
  }
  setOptions(t) {
    var n;
    const r = this.options;
    this.options = v(this, Le).defaultMutationOptions(t), kr(this.options, r) || v(this, Le).getMutationCache().notify({
      type: "observerOptionsUpdated",
      mutation: v(this, pe),
      observer: this
    }), r != null && r.mutationKey && this.options.mutationKey && Zs(r.mutationKey) !== Zs(this.options.mutationKey) ? this.reset() : ((n = v(this, pe)) == null ? void 0 : n.state.status) === "pending" && v(this, pe).setOptions(this.options);
  }
  onUnsubscribe() {
    var t;
    this.hasListeners() || (t = v(this, pe)) == null || t.removeObserver(this);
  }
  onMutationUpdate(t) {
    ee(this, Fe, ur).call(this), ee(this, Fe, Pn).call(this, t);
  }
  getCurrentResult() {
    return v(this, qe);
  }
  reset() {
    var t;
    (t = v(this, pe)) == null || t.removeObserver(this), I(this, pe, void 0), ee(this, Fe, ur).call(this), ee(this, Fe, Pn).call(this);
  }
  mutate(t, r) {
    var n;
    return I(this, Ie, r), (n = v(this, pe)) == null || n.removeObserver(this), I(this, pe, v(this, Le).getMutationCache().build(v(this, Le), this.options)), v(this, pe).addObserver(this), v(this, pe).execute(t);
  }
}, Le = new WeakMap(), qe = new WeakMap(), pe = new WeakMap(), Ie = new WeakMap(), Fe = new WeakSet(), ur = function() {
  var r;
  const t = ((r = v(this, pe)) == null ? void 0 : r.state) ?? _u();
  I(this, qe, {
    ...t,
    isPending: t.status === "pending",
    isSuccess: t.status === "success",
    isError: t.status === "error",
    isIdle: t.status === "idle",
    mutate: this.mutate,
    reset: this.reset
  });
}, Pn = function(t) {
  $r.batch(() => {
    var r, n, s, a, i, l, u, d;
    if (v(this, Ie) && this.hasListeners()) {
      const c = v(this, qe).variables, f = v(this, qe).context, y = {
        client: v(this, Le),
        meta: this.options.meta,
        mutationKey: this.options.mutationKey
      };
      if ((t == null ? void 0 : t.type) === "success") {
        try {
          (n = (r = v(this, Ie)).onSuccess) == null || n.call(
            r,
            t.data,
            c,
            f,
            y
          );
        } catch (m) {
          Promise.reject(m);
        }
        try {
          (a = (s = v(this, Ie)).onSettled) == null || a.call(
            s,
            t.data,
            null,
            c,
            f,
            y
          );
        } catch (m) {
          Promise.reject(m);
        }
      } else if ((t == null ? void 0 : t.type) === "error") {
        try {
          (l = (i = v(this, Ie)).onError) == null || l.call(
            i,
            t.error,
            c,
            f,
            y
          );
        } catch (m) {
          Promise.reject(m);
        }
        try {
          (d = (u = v(this, Ie)).onSettled) == null || d.call(
            u,
            void 0,
            t.error,
            c,
            f,
            y
          );
        } catch (m) {
          Promise.reject(m);
        }
      }
    }
    this.listeners.forEach((c) => {
      c(v(this, qe));
    });
  });
}, fo), Ou = ae.createContext(
  void 0
), Br = (e) => {
  const t = ae.useContext(Ou);
  if (!t)
    throw new Error("No QueryClient set, use QueryClientProvider to set one");
  return t;
}, Ca = ae.createContext(!1), Tu = () => ae.useContext(Ca);
Ca.Provider;
function Cu() {
  let e = !1;
  return {
    clearReset: () => {
      e = !1;
    },
    reset: () => {
      e = !0;
    },
    isReset: () => e
  };
}
var Au = ae.createContext(Cu()), Du = () => ae.useContext(Au), Mu = (e, t) => {
  (e.suspense || e.throwOnError || e.experimental_prefetchInRender) && (t.isReset() || (e.retryOnMount = !1));
}, Pu = (e) => {
  ae.useEffect(() => {
    e.clearReset();
  }, [e]);
}, Lu = ({
  result: e,
  errorResetBoundary: t,
  throwOnError: r,
  query: n,
  suspense: s
}) => e.isError && !t.isReset() && !e.isFetching && n && (s && e.data === void 0 || Oa(r, [e.error, n])), Iu = (e) => {
  if (e.suspense) {
    const r = (s) => s === "static" ? s : Math.max(s ?? 1e3, 1e3), n = e.staleTime;
    e.staleTime = typeof n == "function" ? (...s) => r(n(...s)) : r(n), typeof e.gcTime == "number" && (e.gcTime = Math.max(
      e.gcTime,
      1e3
    ));
  }
}, Fu = (e, t) => e.isLoading && e.isFetching && !t, Uu = (e, t) => (e == null ? void 0 : e.suspense) && t.isPending, ao = (e, t, r) => t.fetchOptimistic(e).catch(() => {
  r.clearReset();
});
function $u(e, t, r) {
  var f, y, m, p, b;
  if (process.env.NODE_ENV !== "production" && (typeof e != "object" || Array.isArray(e)))
    throw new Error(
      'Bad argument type. Starting with v5, only the "Object" form is allowed when calling query related functions. Please use the error stack to find the culprit call. More info here: https://tanstack.com/query/latest/docs/react/guides/migrating-to-v5#supports-a-single-signature-one-object'
    );
  const n = Tu(), s = Du(), a = Br(), i = a.defaultQueryOptions(e);
  (y = (f = a.getDefaultOptions().queries) == null ? void 0 : f._experimental_beforeQuery) == null || y.call(
    f,
    i
  ), process.env.NODE_ENV !== "production" && (i.queryFn || console.error(
    `[${i.queryHash}]: No queryFn was passed as an option, and no default queryFn was found. The queryFn parameter is only optional when using a default queryFn. More info here: https://tanstack.com/query/latest/docs/framework/react/guides/default-query-function`
  )), i._optimisticResults = n ? "isRestoring" : "optimistic", Iu(i), Mu(i, s), Pu(s);
  const l = !a.getQueryCache().get(i.queryHash), [u] = ae.useState(
    () => new t(
      a,
      i
    )
  ), d = u.getOptimisticResult(i), c = !n && e.subscribed !== !1;
  if (ae.useSyncExternalStore(
    ae.useCallback(
      (g) => {
        const E = c ? u.subscribe($r.batchCalls(g)) : Er;
        return u.updateResult(), E;
      },
      [u, c]
    ),
    () => u.getCurrentResult(),
    () => u.getCurrentResult()
  ), ae.useEffect(() => {
    u.setOptions(i);
  }, [i, u]), Uu(i, d))
    throw ao(i, u, s);
  if (Lu({
    result: d,
    errorResetBoundary: s,
    throwOnError: i.throwOnError,
    query: a.getQueryCache().get(i.queryHash),
    suspense: i.suspense
  }))
    throw d.error;
  if ((p = (m = a.getDefaultOptions().queries) == null ? void 0 : m._experimental_afterQuery) == null || p.call(
    m,
    i,
    d
  ), i.experimental_prefetchInRender && !Ft && Fu(d, n)) {
    const g = l ? (
      // Fetch immediately on render in order to ensure `.promise` is resolved even if the component is unmounted
      ao(i, u, s)
    ) : (
      // subscribe to the "cache promise" so that we can finalize the currentThenable once data comes in
      (b = a.getQueryCache().get(i.queryHash)) == null ? void 0 : b.promise
    );
    g == null || g.catch(Er).finally(() => {
      u.updateResult();
    });
  }
  return i.notifyOnChangeProps ? d : u.trackResult(d);
}
function Qn(e, t) {
  return $u(e, Nu);
}
function fr(e, t) {
  const r = Br(), [n] = ae.useState(
    () => new Ru(
      r,
      e
    )
  );
  ae.useEffect(() => {
    n.setOptions(e);
  }, [n, e]);
  const s = ae.useSyncExternalStore(
    ae.useCallback(
      (i) => n.subscribe($r.batchCalls(i)),
      [n]
    ),
    () => n.getCurrentResult(),
    () => n.getCurrentResult()
  ), a = ae.useCallback(
    (i, l) => {
      n.mutate(i, l).catch(Er);
    },
    [n]
  );
  if (s.error && Oa(n.options.throwOnError, [s.error]))
    throw s.error;
  return { ...s, mutate: a, mutateAsync: s.mutate };
}
function Bu(e, t, r, n) {
  return Qn({
    queryKey: e,
    queryFn: async () => await we.get(t, { params: r }),
    ...n
  });
}
function Vu(e, t, r, n) {
  return Qn({
    queryKey: [...e, r],
    queryFn: async () => await we.get(t, {
      params: r
    }),
    ...n
  });
}
function dh(e, t = "post", r) {
  return fr({
    mutationFn: async (n) => {
      switch (t) {
        case "post":
          return we.post(e, n);
        case "put":
          return we.put(e, n);
        case "patch":
          return we.patch(e, n);
        case "delete":
          return we.delete(e);
      }
    },
    ...r
  });
}
function uh(e, t) {
  const r = Br(), n = (d) => Vu(
    [t, "list"],
    e,
    d
  ), s = (d, c) => Bu(
    [t, d],
    `${e}/${d}`,
    void 0,
    {
      enabled: !!d,
      ...c
    }
  ), a = (d) => fr({
    mutationFn: (c) => we.post(e, c),
    onSuccess: () => {
      r.invalidateQueries({ queryKey: [t] });
    },
    ...d
  }), i = (d) => fr({
    mutationFn: ({ id: c, data: f }) => we.put(`${e}/${c}`, f),
    onSuccess: (c, f) => {
      r.invalidateQueries({ queryKey: [t, f.id] }), r.invalidateQueries({ queryKey: [t, "list"] });
    },
    ...d
  }), l = (d) => fr({
    mutationFn: (c) => we.delete(`${e}/${c}`),
    onSuccess: () => {
      r.invalidateQueries({ queryKey: [t] });
    },
    ...d
  }), u = B(() => {
    r.invalidateQueries({ queryKey: [t] });
  }, [r]);
  return {
    useList: n,
    useOne: s,
    useCreate: a,
    useUpdate: i,
    useDelete: l,
    invalidate: u
  };
}
function fh() {
  const e = Br();
  return B(
    async (t, r, n) => {
      await e.prefetchQuery({
        queryKey: t,
        queryFn: () => we.get(r, { params: n }),
        staleTime: 5 * 60 * 1e3
        // 5 minutes
      });
    },
    [e]
  );
}
function hh(e, t, r = 20) {
  return Qn({
    queryKey: e,
    queryFn: async () => await we.get(t, {
      params: { pageSize: r }
    })
  });
}
export {
  yi as AdminLayout,
  Vd as ApiClient,
  ch as AppRouter,
  Wd as AuthProvider,
  Xu as Avatar,
  Zu as AvatarGroup,
  Yu as Badge,
  wi as Button,
  Si as Card,
  Ri as CardBody,
  Oi as CardFooter,
  _i as CardHeader,
  zo as DEFAULT_API_CONFIG,
  Gl as DEFAULT_AUTH_CONFIG,
  Js as DashboardPage,
  Gu as DataTable,
  ef as Dropdown,
  nn as ERROR_CODES,
  oe as EVENTS,
  Os as HTTP_STATUS,
  gi as Header,
  ki as Input,
  dn as LiveMetricCard,
  su as LiveOperationsDashboard,
  th as LiveOperationsPage,
  Ks as LoginPage,
  Ci as Modal,
  Qs as NotFoundPage,
  nf as PAGINATION_DEFAULTS,
  Sa as ProtectedRoute,
  rf as QUERY_KEYS,
  Q as STORAGE_KEYS,
  tf as Select,
  mi as Sidebar,
  ja as Sparkline,
  Na as TIME_FRAME_OPTIONS,
  Xd as TimeFrameSelector,
  Qu as ToastProvider,
  bf as addNotification,
  we as apiClient,
  rc as authReducer,
  tc as clearAuth,
  lf as clearAuthError,
  wf as clearNotifications,
  dc as createAppStore,
  Lf as dispatch,
  sn as fetchCurrentUser,
  Pf as getState,
  kf as hideLoading,
  Dt as login,
  ir as logout,
  vr as refreshAccessToken,
  vf as removeNotification,
  Jo as selectAccessToken,
  Ko as selectAuthError,
  qo as selectAuthIsLoading,
  Df as selectBreadcrumbs,
  Qo as selectExpiresAt,
  df as selectHasPermission,
  uf as selectHasRole,
  Ho as selectIsAuthenticated,
  _f as selectLanguage,
  Af as selectLoadingMessage,
  Tf as selectNotifications,
  Mf as selectPageTitle,
  Rf as selectSidebarCollapsed,
  Of as selectSidebarMobileOpen,
  Sf as selectTheme,
  Cf as selectUiIsLoading,
  Vn as selectUser,
  cf as sessionExpired,
  af as setAuthError,
  Nf as setBreadcrumbs,
  pf as setLanguage,
  xf as setMobileSidebarOpen,
  jf as setPageTitle,
  gf as setSidebarCollapsed,
  ff as setTheme,
  of as setTokens,
  sf as setUser,
  Ef as showLoading,
  zn as store,
  yf as toggleMobileSidebar,
  mf as toggleSidebar,
  hf as toggleThemeMode,
  ic as uiReducer,
  ec as updateLastActivity,
  Zl as updateUser,
  dh as useApiMutation,
  Bu as useApiQuery,
  uc as useAppDispatch,
  xe as useAppSelector,
  Fr as useAuth,
  ka as useAuthContext,
  oh as useAuthError,
  sh as useAuthLoading,
  uh as useCrudApi,
  nh as useCurrentUser,
  hh as useInfiniteApi,
  rh as useIsAuthenticated,
  ru as useLiveOperations,
  Vu as usePaginatedQuery,
  ah as usePermission,
  fh as usePrefetch,
  lh as useRequireAuth,
  ih as useRole,
  Ju as useToast
};
