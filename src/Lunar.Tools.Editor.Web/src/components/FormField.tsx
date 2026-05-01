import type { CSSProperties, ReactNode } from 'react'

interface Props {
  label: string
  children: ReactNode
  style?: CSSProperties
}

export function FormField({ label, children, style }: Props) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 4, ...style }}>
      <label>{label}</label>
      {children}
    </div>
  )
}

interface RowProps {
  children: ReactNode
  gap?: number
}

export function FormRow({ children, gap = 12 }: RowProps) {
  return (
    <div style={{ display: 'flex', gap, flexWrap: 'wrap' }}>
      {children}
    </div>
  )
}

interface SectionProps {
  title: string
  children: ReactNode
}

export function FormSection({ title, children }: SectionProps) {
  return (
    <fieldset style={sectionStyle}>
      <legend style={legendStyle}>{title}</legend>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {children}
      </div>
    </fieldset>
  )
}

const sectionStyle: CSSProperties = {
  border: '1px solid var(--border)',
  borderRadius: 4,
  padding: '10px 14px',
  marginBottom: 0,
}

const legendStyle: CSSProperties = {
  fontSize: 11,
  fontWeight: 600,
  color: 'var(--text-dim)',
  textTransform: 'uppercase',
  letterSpacing: '0.5px',
  padding: '0 4px',
}
