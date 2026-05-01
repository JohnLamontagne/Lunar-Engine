import { api } from '../../api/client'
import type { ValidationIssue } from '../../api/client'
import { useEditorStore } from '../../store'

const SEVERITY_COLOR: Record<string, string> = {
  error: 'var(--error)',
  warning: 'var(--warning)',
  info: 'var(--info)',
}

function IssueRow({ issue, onNavigate }: { issue: ValidationIssue; onNavigate: (issue: ValidationIssue) => void }) {
  return (
    <div style={styles.row} onClick={() => onNavigate(issue)}>
      <span style={{ ...styles.severity, color: SEVERITY_COLOR[issue.severity] ?? 'var(--text-dim)' }}>
        {issue.severity.toUpperCase()[0]}
      </span>
      <span style={styles.location}>
        {issue.fileName}:{issue.line}:{issue.column}
      </span>
      <span style={styles.id}>{issue.diagnosticId}</span>
      <span style={styles.message}>{issue.message}</span>
    </div>
  )
}

export function DiagnosticsPanel() {
  const { diagnostics, openTabs, openScriptTab } = useEditorStore()
  if (diagnostics.length === 0) return null

  async function handleNavigate(issue: ValidationIssue) {
    if (!issue.filePath) return
    const existing = openTabs.find((t) => t.filePath === issue.filePath)
    if (!existing) {
      try {
        const doc = await api.scripts.load(issue.filePath)
        openScriptTab(doc)
      } catch {
        // file may not be loadable
      }
    }
  }

  const errors = diagnostics.filter((d) => d.severity === 'error').length
  const warnings = diagnostics.filter((d) => d.severity === 'warning').length

  return (
    <div style={styles.panel}>
      <div style={styles.header}>
        <span>Problems</span>
        {errors > 0 && <span style={{ color: 'var(--error)' }}>{errors} error{errors !== 1 ? 's' : ''}</span>}
        {warnings > 0 && <span style={{ color: 'var(--warning)' }}>{warnings} warning{warnings !== 1 ? 's' : ''}</span>}
      </div>
      <div style={styles.list}>
        {diagnostics.map((issue, i) => (
          <IssueRow key={i} issue={issue} onNavigate={handleNavigate} />
        ))}
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  panel: {
    background: 'var(--bg-panel)',
    borderTop: '1px solid var(--border)',
    maxHeight: 180,
    display: 'flex',
    flexDirection: 'column',
    flexShrink: 0,
  },
  header: {
    display: 'flex',
    gap: 12,
    alignItems: 'center',
    padding: '4px 12px',
    borderBottom: '1px solid var(--border)',
    fontSize: 11,
    fontWeight: 600,
    textTransform: 'uppercase',
    letterSpacing: '0.5px',
    color: 'var(--text-dim)',
    flexShrink: 0,
  },
  list: {
    overflowY: 'auto',
    flex: 1,
  },
  row: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
    padding: '3px 12px',
    cursor: 'pointer',
    borderBottom: '1px solid transparent',
    fontSize: 12,
    fontFamily: 'var(--font-mono)',
  },
  severity: { width: 14, fontWeight: 700, flexShrink: 0 },
  location: { color: 'var(--text-dim)', flexShrink: 0, minWidth: 140 },
  id: { color: 'var(--text-dim)', flexShrink: 0, minWidth: 60 },
  message: { flex: 1, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' },
}
