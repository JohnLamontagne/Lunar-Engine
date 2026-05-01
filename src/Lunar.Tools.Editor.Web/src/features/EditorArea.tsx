import { useEditorStore } from '../store'
import { ScriptEditor } from './scripts/ScriptEditor'
import { ItemEditor } from './items/ItemEditor'
import { SpellEditor } from './spells/SpellEditor'

/** Tab bar shared across all editor types, plus the active editor dispatched by nodeType. */
export function EditorArea() {
  const { openTabs, activeTabPath, closeTab, setActiveTab } = useEditorStore()

  const activeTab = openTabs.find((t) => t.filePath === activeTabPath)

  if (openTabs.length === 0) {
    return (
      <div style={styles.empty}>
        <span style={{ color: 'var(--text-dim)' }}>
          Open a file from the project tree to start editing
        </span>
      </div>
    )
  }

  return (
    <div style={styles.container}>
      {/* Shared tab bar */}
      <div style={styles.tabBar}>
        {openTabs.map((tab) => (
          <div
            key={tab.filePath}
            style={{
              ...styles.tab,
              ...(tab.filePath === activeTabPath ? styles.tabActive : {}),
            }}
            onClick={() => setActiveTab(tab.filePath)}
          >
            <span style={styles.tabLabel}>
              {tab.dirty ? '● ' : ''}{tab.label}
            </span>
            <button
              style={styles.closeBtn}
              onClick={(e) => { e.stopPropagation(); closeTab(tab.filePath) }}
              title="Close"
            >
              ×
            </button>
          </div>
        ))}
      </div>

      {/* Active editor */}
      <div style={styles.editorWrap}>
        {activeTab?.nodeType === 'script' && <ScriptEditor filePath={activeTab.filePath} />}
        {activeTab?.nodeType === 'item'   && <ItemEditor   filePath={activeTab.filePath} />}
        {activeTab?.nodeType === 'spell'  && <SpellEditor  filePath={activeTab.filePath} />}
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  container: { flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' },
  empty: {
    flex: 1,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontSize: 14,
  },
  tabBar: {
    display: 'flex',
    alignItems: 'center',
    background: 'var(--bg-panel)',
    borderBottom: '1px solid var(--border)',
    height: 35,
    overflowX: 'auto',
    flexShrink: 0,
  },
  tab: {
    display: 'flex',
    alignItems: 'center',
    gap: 4,
    padding: '0 12px',
    height: '100%',
    cursor: 'pointer',
    borderRight: '1px solid var(--border)',
    color: 'var(--text-dim)',
    whiteSpace: 'nowrap',
    flexShrink: 0,
  },
  tabActive: {
    background: 'var(--bg-base)',
    color: 'var(--text-bright)',
    borderBottom: '1px solid var(--accent)',
  },
  tabLabel: { fontSize: 12 },
  closeBtn: {
    background: 'transparent',
    padding: '0 2px',
    fontSize: 14,
    lineHeight: 1,
    color: 'var(--text-dim)',
    minWidth: 16,
  },
  editorWrap: { flex: 1, overflow: 'hidden', display: 'flex', flexDirection: 'column' },
}
