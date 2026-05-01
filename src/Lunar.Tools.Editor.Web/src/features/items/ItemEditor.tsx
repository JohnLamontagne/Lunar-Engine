import { useState } from 'react'
import { api, type ItemEditorDocument } from '../../api/client'
import { FormField, FormRow, FormSection } from '../../components/FormField'
import { useEditorStore } from '../../store'

const ITEM_TYPES = ['NA', 'Equipment', 'Usable'] as const
const SLOT_TYPES = ['NE', 'MainArm', 'SideArm', 'Ring', 'SecRing', 'Helm', 'Boots', 'Chest', 'Legs', 'Shoulder'] as const

interface Props {
  filePath: string
}

export function ItemEditor({ filePath }: Props) {
  const { openTabs, updateTabContent, markTabSaved } = useEditorStore()
  const tab = openTabs.find((t) => t.filePath === filePath)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  if (!tab) return null

  const doc: ItemEditorDocument = JSON.parse(tab.content)

  function update(patch: Partial<ItemEditorDocument>) {
    const next = { ...doc, ...patch }
    updateTabContent(filePath, JSON.stringify(next, null, 2))
  }

  function num(val: string): number {
    const n = parseInt(val, 10)
    return isNaN(n) ? 0 : n
  }

  async function handleSave() {
    setSaving(true)
    setError(null)
    try {
      await api.items.save(doc)
      markTabSaved(filePath)
    } catch (e) {
      setError(String(e))
    } finally {
      setSaving(false)
    }
  }

  return (
    <div style={styles.container}>
      <div style={styles.toolbar}>
        <span style={styles.filename}>{tab.label}{tab.dirty ? ' ●' : ''}</span>
        <div style={{ flex: 1 }} />
        {error && <span style={styles.error}>{error}</span>}
        <button className="primary" onClick={handleSave} disabled={saving || !tab.dirty}>
          {saving ? 'Saving…' : 'Save'}
        </button>
      </div>

      <div style={styles.form}>
        <FormSection title="Identity">
          <FormRow>
            <FormField label="Name" style={{ flex: 2 }}>
              <input value={doc.name} onChange={(e) => update({ name: e.target.value })} />
            </FormField>
            <FormField label="Sprite Name" style={{ flex: 2 }}>
              <input value={doc.spriteName} onChange={(e) => update({ spriteName: e.target.value })} />
            </FormField>
            <FormField label="Behavior Key" style={{ flex: 2 }}>
              <input value={doc.behaviorKey} onChange={(e) => update({ behaviorKey: e.target.value })} />
            </FormField>
          </FormRow>
          <FormRow>
            <FormField label="Item Type" style={{ flex: 1 }}>
              <select
                value={doc.itemType}
                onChange={(e) => update({ itemType: e.target.value as ItemEditorDocument['itemType'] })}
                style={selectStyle}
              >
                {ITEM_TYPES.map((t) => <option key={t}>{t}</option>)}
              </select>
            </FormField>
            <FormField label="Slot Type" style={{ flex: 2 }}>
              <select
                value={doc.slotType}
                onChange={(e) => update({ slotType: e.target.value as ItemEditorDocument['slotType'] })}
                style={selectStyle}
              >
                {SLOT_TYPES.map((t) => <option key={t}>{t}</option>)}
              </select>
            </FormField>
            <FormField label="Stackable" style={{ flex: 1, justifyContent: 'center' }}>
              <input
                type="checkbox"
                checked={doc.stackable}
                onChange={(e) => update({ stackable: e.target.checked })}
                style={{ width: 'auto', marginTop: 6 }}
              />
            </FormField>
          </FormRow>
        </FormSection>

        <FormSection title="Stat Modifiers">
          <FormRow>
            {(['strength', 'intelligence', 'dexterity', 'defence', 'health'] as const).map((stat) => (
              <FormField key={stat} label={stat.charAt(0).toUpperCase() + stat.slice(1)} style={{ flex: 1 }}>
                <input
                  type="number"
                  value={doc[stat]}
                  onChange={(e) => update({ [stat]: num(e.target.value) })}
                />
              </FormField>
            ))}
          </FormRow>
        </FormSection>
      </div>
    </div>
  )
}

const selectStyle: React.CSSProperties = {
  fontFamily: 'var(--font)',
  fontSize: 12,
  background: 'var(--bg-base)',
  color: 'var(--text)',
  border: '1px solid var(--border)',
  borderRadius: 3,
  padding: '4px 8px',
  width: '100%',
}

const styles: Record<string, React.CSSProperties> = {
  container: { flex: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' },
  toolbar: {
    display: 'flex',
    alignItems: 'center',
    gap: 10,
    padding: '6px 14px',
    borderBottom: '1px solid var(--border)',
    background: 'var(--bg-panel)',
    flexShrink: 0,
  },
  filename: { fontFamily: 'var(--font-mono)', fontSize: 12, color: 'var(--text-dim)' },
  error: { color: 'var(--error)', fontSize: 12 },
  form: {
    flex: 1,
    overflowY: 'auto',
    padding: 16,
    display: 'flex',
    flexDirection: 'column',
    gap: 14,
  },
}
